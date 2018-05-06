using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using OpenCVForUnity;

namespace CVVTuber
{

    public class VideoCaptureMatSourceGetter : MatSource
    {

        /// <summary>
        /// The video capture.
        /// </summary>
        VideoCapture capture;

        Mat returnMat;

        bool didUpdateResultMat;

        public string videoFileName = "dance.avi";

        /// <summary>
        /// The couple_avi_filepath.
        /// </summary>
        string couple_avi_filepath;

        /// <summary>
        /// Indicates whether the video frame needs updating.
        /// </summary>
        bool shouldUpdateVideoFrame = false;

        /// <summary>
        /// The prev frame tick count.
        /// </summary>
        //long prevFrameTickCount;

        /// <summary>
        /// The current frame tick count.
        /// </summary>
        //long currentFrameTickCount;


        #if UNITY_WEBGL && !UNITY_EDITOR
        Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();
#endif

        public override string GetDescription ()
        {
            return "Get MatSource from VideoCapture.";
        }

        // Use this for initialization
        public override void Setup ()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var getFilePath_Coroutine = GetFilePath ();
            coroutines.Push (getFilePath_Coroutine);
            StartCoroutine (getFilePath_Coroutine);
#else
            couple_avi_filepath = OpenCVForUnity.Utils.getFilePath (videoFileName);
            Run ();
#endif


            didUpdateResultMat = false;
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        private IEnumerator GetFilePath ()
        {

            var getFilePathAsync_couple_avi_filepath_Coroutine = OpenCVForUnity.Utils.getFilePathAsync (videoFileName, (result) => {
                couple_avi_filepath = result;
            });
            coroutines.Push (getFilePathAsync_couple_avi_filepath_Coroutine);
            yield return StartCoroutine (getFilePathAsync_couple_avi_filepath_Coroutine);

            coroutines.Clear ();

            Run ();
        }
#endif


        private void Run ()
        {
            returnMat = new Mat ();

            capture = new VideoCapture ();
            capture.open (couple_avi_filepath);

            if (capture.isOpened ()) {
                Debug.Log ("capture.isOpened() true");
            } else {
                Debug.Log ("capture.isOpened() false");
            }


            Debug.Log ("CAP_PROP_FORMAT: " + capture.get (Videoio.CAP_PROP_FORMAT));
            Debug.Log ("CV_CAP_PROP_PREVIEW_FORMAT: " + capture.get (Videoio.CV_CAP_PROP_PREVIEW_FORMAT));
            Debug.Log ("CAP_PROP_POS_MSEC: " + capture.get (Videoio.CAP_PROP_POS_MSEC));
            Debug.Log ("CAP_PROP_POS_FRAMES: " + capture.get (Videoio.CAP_PROP_POS_FRAMES));
            Debug.Log ("CAP_PROP_POS_AVI_RATIO: " + capture.get (Videoio.CAP_PROP_POS_AVI_RATIO));
            Debug.Log ("CAP_PROP_FRAME_COUNT: " + capture.get (Videoio.CAP_PROP_FRAME_COUNT));
            Debug.Log ("CAP_PROP_FPS: " + capture.get (Videoio.CAP_PROP_FPS));
            Debug.Log ("CAP_PROP_FRAME_WIDTH: " + capture.get (Videoio.CAP_PROP_FRAME_WIDTH));
            Debug.Log ("CAP_PROP_FRAME_HEIGHT: " + capture.get (Videoio.CAP_PROP_FRAME_HEIGHT));

            capture.grab ();
            capture.retrieve (returnMat, 0);
            capture.set (Videoio.CAP_PROP_POS_FRAMES, 0);


            StartCoroutine ("WaitFrameTime");
        }

        // Update is called once per frame
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

                //error PlayerLoop called recursively! on iOS.reccomend WebCamTexture.
                if (capture.grab ()) {

                    capture.retrieve (returnMat, 0);

                    Imgproc.cvtColor (returnMat, returnMat, Imgproc.COLOR_BGR2RGB);

                    didUpdateResultMat = true;
                }

            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        public override void Dispose ()
        {
            Debug.Log ("VideoCaptureMatSource Dispose");

            StopCoroutine ("WaitFrameTime");

            if (capture != null)
                capture.release ();

            if (returnMat != null)
                returnMat.Dispose ();

#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var coroutine in coroutines) {
                StopCoroutine (coroutine);
                ((IDisposable)coroutine).Dispose ();
            }
#endif
        }

        private IEnumerator WaitFrameTime ()
        {
            double videoFPS = (capture.get (Videoio.CAP_PROP_FPS) <= 0) ? 10.0 : capture.get (Videoio.CAP_PROP_FPS);
            int frameTime_msec = (int)Math.Round (1000.0 / videoFPS);

            while (true) {
                shouldUpdateVideoFrame = true;

                //prevFrameTickCount = currentFrameTickCount;
                //currentFrameTickCount = Core.getTickCount ();

                yield return new WaitForSeconds (frameTime_msec / 1000f);
            }
        }

        public override Mat GetMatSource ()
        {
            if (didUpdateResultMat) {
                return returnMat;
            } else {
                return null;
            }
        }
    }
}