using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace CVVTuber
{
    public class OpenCVFaceRectGetter : CVVTuberProcess
    {
        public string openCVCascadeFileName;

        public MatSourceGetter matSourceGetter;

        [Header ("Debug")]

        public RawImage screen;

        public bool isDebugMode;

        public bool hideImage;

        Mat debugMat;

        Texture2D debugTexture;

        Color32[] debugColors;

        UnityEngine.Rect faceRect;

        bool didUpdateFaceRect;

        /// <summary>
        /// The gray mat.
        /// </summary>
        Mat grayMat;

        /// <summary>
        /// The cascade.
        /// </summary>
        CascadeClassifier cascade;

        /// <summary>
        /// The faces.
        /// </summary>
        MatOfRect faces;

        /// <summary>
        /// The preset cascade file name.
        /// </summary>
        string openCVCascadeFileNamePreset = "haarcascade_frontalface_alt.xml";

        /// <summary>
        /// The dlib shape predictor file path.
        /// </summary>
        string openCVCascadeFilePath;

        #if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
        #endif

        public override string GetDescription ()
        {
            return "Get face rect from MatSourceGetter.";
        }
            
        public override void Setup ()
        {
            if (string.IsNullOrEmpty(openCVCascadeFileName))
                openCVCascadeFileName = openCVCascadeFileNamePreset;

            #if UNITY_WEBGL && !UNITY_EDITOR
            var getFilePathAsync_Coroutine = OpenCVForUnity.Utils.getFilePathAsync (openCVCascadeFileName, (result) => {
                coroutines.Clear ();

                openCVCascadeFilePath = result;
                Run ();
            });
            coroutines.Push (getFilePathAsync_Coroutine);
            StartCoroutine (getFilePathAsync_Coroutine);
            #else
            openCVCascadeFilePath = OpenCVForUnity.Utils.getFilePath (openCVCascadeFileName);
            Run ();
            #endif
        }

        protected virtual void Run ()
        {
            cascade = new CascadeClassifier ();
            cascade.load (openCVCascadeFilePath);
            #if !UNITY_WSA_10_0
            if (cascade.empty ()) {
                Debug.LogError ("cascade file is not loaded.Please copy from “OpenCVForUnity/StreamingAssets/” to “Assets/StreamingAssets/” folder. ");
            }
            #endif

            grayMat = new Mat ();
            faces = new MatOfRect ();

            didUpdateFaceRect = false;
        }

        public override void UpdateValue ()
        {
            if (cascade == null)
                return;

            if (matSourceGetter == null)
                return;

            didUpdateFaceRect = false;

            Mat rgbaMat = matSourceGetter.GetMatSource ();
            if (rgbaMat != null) {
                if (isDebugMode && screen != null) {

                    if (debugMat != null && (debugMat.width () != rgbaMat.width () || debugMat.height () != rgbaMat.height ())) {
                        debugMat.Dispose ();
                        debugMat = null;
                    }
                    debugMat = debugMat ?? new Mat (rgbaMat.rows (), rgbaMat.cols (), rgbaMat.type ());

                    if (hideImage) {
                        debugMat.setTo (new Scalar(0, 0, 0, 255));
                    } else {
                        rgbaMat.copyTo (debugMat);
                    }

                    if (debugTexture != null && (debugTexture.width != debugMat.width () || debugTexture.height != debugMat.height ())) {
                        Texture2D.Destroy (debugTexture);
                        debugTexture = null;
                    }
                    if (debugTexture == null) {
                        debugTexture = new Texture2D (debugMat.width (), debugMat.height (), TextureFormat.RGBA32, false, false);

                        Vector2 size = screen.rectTransform.sizeDelta;
                        screen.rectTransform.sizeDelta = new Vector2 (size.x, size.x * (float)debugMat.height () / (float)debugMat.width ());
                    }

                    if (debugColors != null && debugColors.Length != debugMat.width () * debugMat.height ()) {
                        debugColors = new Color32[debugMat.width () * debugMat.height ()];
                    }
                    screen.texture = debugTexture;
                    screen.enabled = true;
                } else {
                    if (screen != null)
                        screen.enabled = false;
                }


                Imgproc.cvtColor (rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
                Imgproc.equalizeHist (grayMat, grayMat);


                if (cascade != null)
                    cascade.detectMultiScale (grayMat, faces, 1.1, 2, 2, // TODO: objdetect.CV_HAAR_SCALE_IMAGE
                        new Size (grayMat.cols () * 0.2, grayMat.rows () * 0.2), new Size ());


                OpenCVForUnity.Rect[] rects = faces.toArray ();
                for (int i = 0; i < rects.Length; i++) {
                    if (i == 0) {

                        faceRect = new UnityEngine.Rect (rects [i].x, rects [i].y, rects [i].width, rects [i].height);

                        didUpdateFaceRect = true;

                        //Debug.Log ("detect faces " + rects [i]);

                        if (isDebugMode && screen != null)
                            Imgproc.rectangle (debugMat, new Point (rects [i].x, rects [i].y), new Point (rects [i].x + rects [i].width, rects [i].y + rects [i].height), new Scalar (255, 0, 0, 255), 2);
                    }
                }

                //Imgproc.putText (debugMat, "W:" + debugMat.width () + " H:" + debugMat.height () + " SO:" + Screen.orientation, new Point (5, debugMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);

                if (isDebugMode && screen != null) {
                    OpenCVForUnity.Utils.matToTexture2D (debugMat, debugTexture, debugColors);
                }
            }
        }

        public override void Dispose ()
        {
            if (grayMat != null) {
                grayMat.Dispose ();
                grayMat = null;
            }

            if (faces != null)
                faces.Dispose ();

            if (debugMat != null) {
                debugMat.Dispose ();
                debugMat = null;
            }

            if (debugTexture != null) {
                Texture2D.Destroy (debugTexture);
                debugTexture = null;
            }

            #if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var coroutine in coroutines) {
                StopCoroutine (coroutine);
                ((IDisposable)coroutine).Dispose ();
            }
            #endif
        }

        public virtual UnityEngine.Rect GetFaceRect ()
        {
            if (didUpdateFaceRect) {
                return faceRect;
            } else {
                #if UNITY_5_5_OR_NEWER
                return UnityEngine.Rect.zero;
                #else
                return new UnityEngine.Rect ();
                #endif
            }
        }
    }
}