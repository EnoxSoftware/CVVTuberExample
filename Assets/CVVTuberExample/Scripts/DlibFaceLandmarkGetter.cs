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

        public MatSource matSource;

        public OpenCVFaceRectGetter openCVFaceRectGetter;

        public RawImage debugRawImage;

        List<Vector2> faceLandmarkPoints;

        bool didUpdateFaceLanmarkPoints;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        Color32[] colors;

        /// <summary>
        /// The face landmark detector.
        /// </summary>
        FaceLandmarkDetector faceLandmarkDetector;

        /// <summary>
        /// The dlib shape predictor file name.
        /// </summary>
        #if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        string dlibShapePredictorFileName = "sp_human_face_68_for_mobile.dat";
        #else
        string dlibShapePredictorFileName = "sp_human_face_68.dat";
        #endif

        /// <summary>
        /// The dlib shape predictor file path.
        /// </summary>
        string dlibShapePredictorFilePath;

        #if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
#endif

        public override string GetDescription ()
        {
            return "Get FaceLandmarkPoints from MatSourceGetter.";
        }

        // Use this for initialization
        public override void Setup ()
        {



#if UNITY_WEBGL && !UNITY_EDITOR
            var getFilePath_Coroutine = DlibFaceLandmarkDetector.Utils.getFilePathAsync (dlibShapePredictorFileName, (result) => {
                coroutines.Clear ();

                dlibShapePredictorFilePath = result;
                Run ();
            });
            coroutines.Push (getFilePath_Coroutine);
            StartCoroutine (getFilePath_Coroutine);
#else
            dlibShapePredictorFilePath = DlibFaceLandmarkDetector.Utils.getFilePath (dlibShapePredictorFileName);
            Run ();
#endif
        }

        private void Run ()
        {


            faceLandmarkDetector = new FaceLandmarkDetector (dlibShapePredictorFilePath);

            didUpdateFaceLanmarkPoints = false;
        }


        // Update is called once per frame
        public override void UpdateValue ()
        {
            if (faceLandmarkDetector == null)
                return;

            if (matSource == null)
                return;

            didUpdateFaceLanmarkPoints = false;

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


                OpenCVForUnityUtils.SetImage (faceLandmarkDetector, rgbaMat);

                if (openCVFaceRectGetter != null) {

                    UnityEngine.Rect faceRect = openCVFaceRectGetter.GetFaceRect ();

#if UNITY_5_5_OR_NEWER
                    if (faceRect != UnityEngine.Rect.zero)
#else
                    if (faceRect != new UnityEngine.Rect ())
#endif
                    {

                        faceRect = new UnityEngine.Rect ((float)faceRect.x, (float)faceRect.y + (float)(faceRect.height * 0.1f), (float)faceRect.width, (float)faceRect.height);

                        //detect landmark points
                        List<Vector2> points = faceLandmarkDetector.DetectLandmark (faceRect);


                        faceLandmarkPoints = points;

                        didUpdateFaceLanmarkPoints = true;

                        OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, points, new Scalar (0, 255, 0, 255), 2);
                    }
                } else {

                    //detect face rects
                    List<UnityEngine.Rect> detectResult = faceLandmarkDetector.Detect ();

                    if (detectResult.Count > 0) {

                        //detect landmark points
                        List<Vector2> points = faceLandmarkDetector.DetectLandmark (detectResult [0]);


                        faceLandmarkPoints = points;

                        didUpdateFaceLanmarkPoints = true;

                        if (debugRawImage != null)
                            OpenCVForUnityUtils.DrawFaceLandmark (rgbaMat, points, new Scalar (0, 255, 0, 255), 2);


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

            if (faceLandmarkDetector != null)
                faceLandmarkDetector.Dispose ();

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


        public List<Vector2> getFaceLanmarkPoints ()
        {
            if (didUpdateFaceLanmarkPoints) {
                return faceLandmarkPoints;
            } else {
                return null;
            }
        }
    }

}
