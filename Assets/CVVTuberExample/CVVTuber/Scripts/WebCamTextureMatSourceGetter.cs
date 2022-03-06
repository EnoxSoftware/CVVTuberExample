using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils.Helper;
using UnityEngine;

namespace CVVTuber
{
    [RequireComponent(typeof(WebCamTextureToMatHelper), typeof(ImageOptimizationHelper))]
    public class WebCamTextureMatSourceGetter : CVVTuberProcess, IMatSourceGetter
    {
        protected WebCamTextureToMatHelper webCamTextureToMatHelper;

        protected ImageOptimizationHelper imageOptimizationHelper;

        protected Mat resultMat;

        protected Mat downScaleResultMat;

        protected bool didUpdateResultMat;


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Get mat source from WebCamTexture.";
        }

        public override void Setup()
        {
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper>();

            webCamTextureToMatHelper.Initialize();

            didUpdateResultMat = false;
        }

        public override void UpdateValue()
        {
            didUpdateResultMat = false;

            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame() && !imageOptimizationHelper.IsCurrentFrameSkipped())
            {

                resultMat = webCamTextureToMatHelper.GetMat();
                downScaleResultMat = imageOptimizationHelper.GetDownScaleMat(resultMat);

                didUpdateResultMat = true;
            }
        }

        public override void Dispose()
        {
            if (webCamTextureToMatHelper != null)
                webCamTextureToMatHelper.Dispose();

            if (imageOptimizationHelper != null)
                imageOptimizationHelper.Dispose();

            if (resultMat != null)
            {
                resultMat.Dispose();
                resultMat = null;
            }
        }

        #endregion


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
            webCamTextureToMatHelper.Play();
        }

        public virtual void Pause()
        {
            webCamTextureToMatHelper.Pause();
        }

        public virtual void Stop()
        {
            webCamTextureToMatHelper.Stop();
        }

        public virtual void ChangeCamera()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            string deviceName = webCamTextureToMatHelper.GetDeviceName();
            int nextCameraIndex = -1;
            for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
            {
                if (WebCamTexture.devices[cameraIndex].name == deviceName)
                {
                    nextCameraIndex = ++cameraIndex % WebCamTexture.devices.Length;
                    break;
                }
            }
            if (nextCameraIndex != -1)
            {
                webCamTextureToMatHelper.requestedDeviceName = nextCameraIndex.ToString();
            }
            else
            {
                webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.requestedIsFrontFacing;
            }
#else
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.requestedIsFrontFacing;
#endif
        }
    }
}
