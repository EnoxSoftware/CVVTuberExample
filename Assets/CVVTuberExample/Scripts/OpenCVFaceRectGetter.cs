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

        public MatSource matSource;

        public RawImage debugRawImage;

        UnityEngine.Rect faceRect;

        bool didUpdateFaceRect;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        Color32[] colors;

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
        /// The dlib shape predictor file name.
        /// </summary>
        string openCVCascadeFileName = "haarcascade_frontalface_alt.xml";

        /// <summary>
        /// The dlib shape predictor file path.
        /// </summary>
        string openCVCascadeFilePath;


        #if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
#endif

        public override string GetDescription ()
        {
            return "Get FaceRect from MatSourceGetter.";
        }

        // Use this for initialization
        public override void Setup ()
        {


#if UNITY_WEBGL && !UNITY_EDITOR
            var getFilePath_Coroutine = OpenCVForUnity.Utils.getFilePathAsync (openCVCascadeFileName, (result) => {
                coroutines.Clear ();

                openCVCascadeFilePath = result;
                Run ();
            });
            coroutines.Push (getFilePath_Coroutine);
            StartCoroutine (getFilePath_Coroutine);
#else
            openCVCascadeFilePath = OpenCVForUnity.Utils.getFilePath (openCVCascadeFileName);
            Run ();
#endif
        }

        private void Run ()
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


        // Update is called once per frame
        public override void UpdateValue ()
        {
            if (cascade == null)
                return;

            if (matSource == null)
                return;

            didUpdateFaceRect = false;

            Mat rgbaMat = matSource.GetMatSource ();
            if (rgbaMat != null) {
                if (debugRawImage != null) {
                    if (texture == null || (texture.width != rgbaMat.width () || texture.height != rgbaMat.height ())) {
                        texture = new Texture2D (rgbaMat.cols (), rgbaMat.rows (), TextureFormat.RGBA32, false);
                    }
                    if (colors == null || colors.Length != rgbaMat.width () * rgbaMat.height ()) {
                        colors = new Color32[rgbaMat.width () * rgbaMat.height ()];
                    }
                    debugRawImage.texture = texture;
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

                        if (debugRawImage != null)
                            Imgproc.rectangle (rgbaMat, new Point (rects [i].x, rects [i].y), new Point (rects [i].x + rects [i].width, rects [i].y + rects [i].height), new Scalar (255, 0, 0, 255), 2);

                    }
                }

                //Imgproc.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);

                if (debugRawImage != null) {
                    OpenCVForUnity.Utils.matToTexture2D (rgbaMat, texture, colors);
                }

            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        public override void Dispose ()
        {

            if (grayMat != null)
                grayMat.Dispose ();

            if (faces != null)
                faces.Dispose ();

            if (texture != null) {
                Texture2D.Destroy (texture);
                texture = null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var coroutine in coroutines) {
                StopCoroutine (coroutine);
                ((IDisposable)coroutine).Dispose ();
            }
#endif
        }

        public UnityEngine.Rect GetFaceRect ()
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
