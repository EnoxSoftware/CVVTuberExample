using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace CVVTuber
{    
    public class DlibFaceLandmarkGetter : CVVTuberProcess
    {
        public string dlibShapePredictorFileName;

        public string dlibShapePredictorMobileFileName;

        public MatSourceGetter matSourceGetter;

        public OpenCVFaceRectGetter openCVFaceRectGetter;

        [Header ("Debug")]

        public RawImage screen;

        public bool isDebugMode;

        public bool hideImage;

        Mat debugMat;

        Texture2D debugTexture;

        Color32[] debugColors;

        List<Vector2> faceLandmarkPoints;

        bool didUpdateFaceLanmarkPoints;

        /// <summary>
        /// The face landmark detector.
        /// </summary>
        FaceLandmarkDetector faceLandmarkDetector;

        /// <summary>
        /// The preset dlib shape predictor file name.
        /// </summary>
        string dlibShapePredictorFileNamePreset = "sp_human_face_68.dat";

        /// <summary>
        /// The preset dlib shape predictor mobile file name.
        /// </summary>
        string dlibShapePredictorMobileFileNamePreset = "sp_human_face_68_for_mobile.dat";

        /// <summary>
        /// The dlib shape predictor file path.
        /// </summary>
        string dlibShapePredictorFilePath;

        #if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
        #endif

        public override string GetDescription ()
        {
            return "Get face landmark points from MatSourceGetter.";
        }
            
        public override void Setup ()
        {
            if (string.IsNullOrEmpty(dlibShapePredictorFileName))
                dlibShapePredictorFileName = dlibShapePredictorFileNamePreset;

            if (string.IsNullOrEmpty(dlibShapePredictorMobileFileName))
                dlibShapePredictorMobileFileName = dlibShapePredictorMobileFileNamePreset;


            #if UNITY_WEBGL && !UNITY_EDITOR
            var getFilePath_Coroutine = DlibFaceLandmarkDetector.Utils.getFilePathAsync (dlibShapePredictorMobileFileName, (result) => {
                coroutines.Clear ();

                dlibShapePredictorFilePath = result;
                Run ();
            });
            coroutines.Push (getFilePath_Coroutine);
            StartCoroutine (getFilePath_Coroutine);
            #else
            #if UNITY_ANDROID || UNITY_IOS
            dlibShapePredictorFilePath = DlibFaceLandmarkDetector.Utils.getFilePath (dlibShapePredictorMobileFileName);
            #else
            dlibShapePredictorFilePath = DlibFaceLandmarkDetector.Utils.getFilePath (dlibShapePredictorFileName);
            #endif
            Run ();
            #endif
        }

        protected virtual void Run ()
        {
            faceLandmarkDetector = new FaceLandmarkDetector (dlibShapePredictorFilePath);

            didUpdateFaceLanmarkPoints = false;
        }
            
        public override void UpdateValue ()
        {
            if (faceLandmarkDetector == null)
                return;

            if (matSourceGetter == null)
                return;

            didUpdateFaceLanmarkPoints = false;

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

                OpenCVForUnityUtils.SetImage (faceLandmarkDetector, rgbaMat);

                if (openCVFaceRectGetter != null) {

                    UnityEngine.Rect faceRect = openCVFaceRectGetter.GetFaceRect ();

                    #if UNITY_5_5_OR_NEWER
                    if (faceRect != UnityEngine.Rect.zero)
                    #else
                    if (faceRect != new UnityEngine.Rect ())
                    #endif
                    {
                        // correct the deviation of the detection result of the face rectangle of OpenCV and Dlib.
                        faceRect = new UnityEngine.Rect ((float)faceRect.x, (float)faceRect.y + (float)(faceRect.height * 0.1f), (float)faceRect.width, (float)faceRect.height);

                        List<Vector2> points = faceLandmarkDetector.DetectLandmark (faceRect);

                        faceLandmarkPoints = points;

                        didUpdateFaceLanmarkPoints = true;

                        if (isDebugMode && screen != null)
                            OpenCVForUnityUtils.DrawFaceLandmark (debugMat, points, new Scalar (0, 255, 0, 255), 2);
                    }
                } else {

                    //detect face rects
                    List<UnityEngine.Rect> detectResult = faceLandmarkDetector.Detect ();

                    if (detectResult.Count > 0) {

                        //detect landmark points
                        List<Vector2> points = faceLandmarkDetector.DetectLandmark (detectResult [0]);

                        faceLandmarkPoints = points;

                        didUpdateFaceLanmarkPoints = true;

                        if (isDebugMode && screen != null)
                            OpenCVForUnityUtils.DrawFaceLandmark (debugMat, points, new Scalar (0, 255, 0, 255), 2);
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
            if (faceLandmarkDetector != null)
                faceLandmarkDetector.Dispose ();

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

        public virtual List<Vector2> getFaceLanmarkPoints ()
        {
            if (didUpdateFaceLanmarkPoints) {
                return faceLandmarkPoints;
            } else {
                return null;
            }
        }
    }
}