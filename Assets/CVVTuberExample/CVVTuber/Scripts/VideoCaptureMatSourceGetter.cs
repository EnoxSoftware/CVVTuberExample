using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.VideoioModule;
using OpenCVForUnity.ImgprocModule;
using VideoCapture = OpenCVForUnity.VideoioModule.VideoCapture;

namespace CVVTuber
{
    [RequireComponent (typeof(ImageOptimizationHelper))]
    public class VideoCaptureMatSourceGetter : MatSourceGetter
    {
        public string videoFileName = "dance.avi";

        /// <summary>
        /// The image optimization helper.
        /// </summary>
        ImageOptimizationHelper imageOptimizationHelper;

        /// <summary>
        /// The video capture.
        /// </summary>
        VideoCapture capture;

        Mat captureMat;

        Mat resultMat;

        bool didUpdateResultMat;

        /// <summary>
        /// The video_file_filepath.
        /// </summary>
        string video_file_filepath;

        /// <summary>
        /// Indicates whether the video frame needs updating.
        /// </summary>
        bool shouldUpdateVideoFrame = false;

        #if UNITY_WEBGL && !UNITY_EDITOR
        IEnumerator getFilePath_Coroutine;
        #endif

        public override string GetDescription ()
        {
            return "Get mat source from VideoCapture.";
        }

        public override void Setup ()
        {
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper> ();

            #if UNITY_WEBGL && !UNITY_EDITOR
            getFilePath_Coroutine = GetFilePath ();
            StartCoroutine (getFilePath_Coroutine);
            #else
            video_file_filepath = OpenCVForUnity.UnityUtils.Utils.getFilePath (videoFileName);
            Run ();
            #endif

            didUpdateResultMat = false;
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        private IEnumerator GetFilePath ()
        {
            var getFilePathAsync_Coroutine = OpenCVForUnity.UnityUtils.Utils.getFilePathAsync (videoFileName, (result) => {
                video_file_filepath = result;
            });
            yield return getFilePathAsync_Coroutine;

            getFilePath_Coroutine = null;

            Run ();
        }
        #endif

        protected virtual void Run ()
        {
            captureMat = new Mat ();
            resultMat = new Mat ();

            capture = new VideoCapture ();
            capture.open (video_file_filepath);

            if (capture.isOpened ()) {
                Debug.Log ("capture.isOpened() true");
            } else {
                Debug.Log ("capture.isOpened() false");
            }

            Debug.Log ("CAP_PROP_FORMAT: " + capture.get (Videoio.CAP_PROP_FORMAT));
            Debug.Log ("CAP_PROP_POS_MSEC: " + capture.get (Videoio.CAP_PROP_POS_MSEC));
            Debug.Log ("CAP_PROP_POS_FRAMES: " + capture.get (Videoio.CAP_PROP_POS_FRAMES));
            Debug.Log ("CAP_PROP_POS_AVI_RATIO: " + capture.get (Videoio.CAP_PROP_POS_AVI_RATIO));
            Debug.Log ("CAP_PROP_FRAME_COUNT: " + capture.get (Videoio.CAP_PROP_FRAME_COUNT));
            Debug.Log ("CAP_PROP_FPS: " + capture.get (Videoio.CAP_PROP_FPS));
            Debug.Log ("CAP_PROP_FRAME_WIDTH: " + capture.get (Videoio.CAP_PROP_FRAME_WIDTH));
            Debug.Log ("CAP_PROP_FRAME_HEIGHT: " + capture.get (Videoio.CAP_PROP_FRAME_HEIGHT));

            capture.grab ();
            capture.retrieve (captureMat, 0);
            capture.set (Videoio.CAP_PROP_POS_FRAMES, 0);

            StartCoroutine ("WaitFrameTime");
        }

        public override void UpdateValue ()
        {
            if (capture == null)
                return;

            didUpdateResultMat = false;

            if (shouldUpdateVideoFrame) {
                shouldUpdateVideoFrame = false;

                //Loop play
                if (capture.get (Videoio.CAP_PROP_POS_FRAMES) >= capture.get (Videoio.CAP_PROP_FRAME_COUNT))
                    capture.set (Videoio.CAP_PROP_POS_FRAMES, 0);

                if (capture.grab () && !imageOptimizationHelper.IsCurrentFrameSkipped ()) {

                    capture.retrieve (captureMat, 0);

                    Imgproc.cvtColor (imageOptimizationHelper.GetDownScaleMat (captureMat), resultMat, Imgproc.COLOR_BGR2RGBA);

                    didUpdateResultMat = true;
                }
            }
        }

        public override void Dispose ()
        {
            StopCoroutine ("WaitFrameTime");

            if (imageOptimizationHelper != null)
                imageOptimizationHelper.Dispose ();
            
            if (capture != null)
                capture.release ();

            if (captureMat != null) {
                captureMat.Dispose ();
                captureMat = null;
            }

            if (resultMat != null) {
                resultMat.Dispose ();
                resultMat = null;
            }

            #if UNITY_WEBGL && !UNITY_EDITOR
            if (getFilePath_Coroutine != null) {
                StopCoroutine (getFilePath_Coroutine);
                ((IDisposable)getFilePath_Coroutine).Dispose ();
            }
            #endif
        }

        private IEnumerator WaitFrameTime ()
        {
            double videoFPS = (capture.get (Videoio.CAP_PROP_FPS) <= 0) ? 10.0 : capture.get (Videoio.CAP_PROP_FPS);
            int frameTime_msec = (int)Math.Round (1000.0 / videoFPS);

            while (true) {
                shouldUpdateVideoFrame = true;

                yield return new WaitForSeconds (frameTime_msec / 1000f);
            }
        }

        public override Mat GetMatSource ()
        {
            if (didUpdateResultMat) {
                return resultMat;
            } else {
                return null;
            }
        }
    }
}