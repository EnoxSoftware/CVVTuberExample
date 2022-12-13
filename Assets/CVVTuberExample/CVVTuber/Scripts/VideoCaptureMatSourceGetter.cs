using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.VideoioModule;
using System;
using System.Collections;
using UnityEngine;
using VideoCapture = OpenCVForUnity.VideoioModule.VideoCapture;

namespace CVVTuber
{
    [RequireComponent(typeof(ImageOptimizationHelper))]
    public class VideoCaptureMatSourceGetter : CVVTuberProcess, IMatSourceGetter
    {
        [Header("[Setting]")]

        public string videoFileName = "DlibFaceLandmarkDetector/dance_mjpeg.mjpeg";

        protected ImageOptimizationHelper imageOptimizationHelper;

        protected VideoCapture capture;

        protected Mat captureMat;

        protected Mat resultMat;

        protected Mat downScaleResultMat;

        protected bool didUpdateResultMat;

        protected string videoFilePath;

        protected bool shouldUpdateVideoFrame;

        protected bool isPausing;

#if UNITY_WEBGL
        protected IEnumerator getFilePath_Coroutine;
#endif


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Get mat source from VideoCapture.";
        }

        public override void Setup()
        {
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper>();

#if UNITY_WEBGL
            getFilePath_Coroutine = GetFilePath();
            StartCoroutine(getFilePath_Coroutine);
#else
            videoFilePath = OpenCVForUnity.UnityUtils.Utils.getFilePath(videoFileName);
            Run();
#endif

            didUpdateResultMat = false;
        }

        public override void UpdateValue()
        {
            if (capture == null)
                return;

            didUpdateResultMat = false;

            if (shouldUpdateVideoFrame)
            {
                shouldUpdateVideoFrame = false;

                //Loop play
                if (capture.get(Videoio.CAP_PROP_POS_FRAMES) >= capture.get(Videoio.CAP_PROP_FRAME_COUNT))
                    capture.set(Videoio.CAP_PROP_POS_FRAMES, 0);

                if (capture.grab() && !imageOptimizationHelper.IsCurrentFrameSkipped())
                {

                    capture.retrieve(captureMat, 0);

                    Imgproc.cvtColor(captureMat, resultMat, Imgproc.COLOR_BGR2RGBA);
                    downScaleResultMat = imageOptimizationHelper.GetDownScaleMat(resultMat);

                    didUpdateResultMat = true;
                }
            }
        }

        public override void Dispose()
        {
            StopCoroutine("WaitFrameTime");

            if (imageOptimizationHelper != null)
                imageOptimizationHelper.Dispose();

            if (capture != null)
                capture.release();

            if (captureMat != null)
            {
                captureMat.Dispose();
                captureMat = null;
            }

            if (resultMat != null)
            {
                resultMat.Dispose();
                resultMat = null;
            }

#if UNITY_WEBGL
            if (getFilePath_Coroutine != null)
            {
                StopCoroutine(getFilePath_Coroutine);
                ((IDisposable)getFilePath_Coroutine).Dispose();
            }
#endif
        }

        #endregion


#if UNITY_WEBGL
        protected virtual IEnumerator GetFilePath()
        {
            var getFilePathAsync_Coroutine = OpenCVForUnity.UnityUtils.Utils.getFilePathAsync(videoFileName, (result) =>
            {
                videoFilePath = result;
            });
            yield return getFilePathAsync_Coroutine;

            getFilePath_Coroutine = null;

            Run();
        }
#endif

        protected virtual void Run()
        {
            if (string.IsNullOrEmpty(videoFilePath))
            {
                Debug.LogError("video file does not exist.");
            }

            captureMat = new Mat();
            resultMat = new Mat();

            capture = new VideoCapture();
            capture.open(videoFilePath);

            if (!capture.isOpened())
            {
                Debug.LogError("capture.isOpened() false");
            }

            //Debug.Log("CAP_PROP_FORMAT: " + capture.get(Videoio.CAP_PROP_FORMAT));
            //Debug.Log("CAP_PROP_POS_MSEC: " + capture.get(Videoio.CAP_PROP_POS_MSEC));
            //Debug.Log("CAP_PROP_POS_FRAMES: " + capture.get(Videoio.CAP_PROP_POS_FRAMES));
            //Debug.Log("CAP_PROP_POS_AVI_RATIO: " + capture.get(Videoio.CAP_PROP_POS_AVI_RATIO));
            //Debug.Log("CAP_PROP_FRAME_COUNT: " + capture.get(Videoio.CAP_PROP_FRAME_COUNT));
            //Debug.Log("CAP_PROP_FPS: " + capture.get(Videoio.CAP_PROP_FPS));
            //Debug.Log("CAP_PROP_FRAME_WIDTH: " + capture.get(Videoio.CAP_PROP_FRAME_WIDTH));
            //Debug.Log("CAP_PROP_FRAME_HEIGHT: " + capture.get(Videoio.CAP_PROP_FRAME_HEIGHT));

            capture.grab();
            capture.retrieve(captureMat, 0);
            capture.set(Videoio.CAP_PROP_POS_FRAMES, 0);

            StartCoroutine("WaitFrameTime");
        }

        protected virtual IEnumerator WaitFrameTime()
        {
            double videoFPS = (capture.get(Videoio.CAP_PROP_FPS) <= 0) ? 10.0 : capture.get(Videoio.CAP_PROP_FPS);
            int frameTime_msec = (int)Math.Round(1000.0 / videoFPS);

            while (true)
            {

                while (isPausing)
                {
                    yield return null;
                }

                shouldUpdateVideoFrame = true;

                yield return new WaitForSeconds(frameTime_msec / 1000f);
            }
        }


        #region IMatSourceGetter

        public virtual Mat GetMatSource()
        {
            if (didUpdateResultMat)
            {
                return resultMat;
            }
            else
            {
                return null;
            }
        }

        public virtual Mat GetDownScaleMatSource()
        {
            if (didUpdateResultMat)
            {
                return downScaleResultMat;
            }
            else
            {
                return null;
            }
        }

        public virtual float GetDownScaleRatio()
        {
            return imageOptimizationHelper.downscaleRatio;
        }

        #endregion


        public virtual void Play()
        {
            isPausing = false;
        }

        public virtual void Stop()
        {
            isPausing = true;
        }
    }
}