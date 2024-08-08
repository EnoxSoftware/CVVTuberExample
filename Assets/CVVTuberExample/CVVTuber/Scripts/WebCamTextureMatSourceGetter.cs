using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils.Helper;
using UnityEngine;

namespace CVVTuber
{
    [RequireComponent(typeof(WebCamTexture2MatHelper), typeof(ImageOptimizationHelper))]
    public class WebCamTextureMatSourceGetter : CVVTuberProcess, IMatSourceGetter
    {
        protected WebCamTexture2MatHelper webCamTexture2MatHelper;

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
            webCamTexture2MatHelper = gameObject.GetComponent<WebCamTexture2MatHelper>();
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper>();

            webCamTexture2MatHelper.Initialize();

            didUpdateResultMat = false;
        }

        public override void UpdateValue()
        {
            if (webCamTexture2MatHelper == null)
                return;
            if (imageOptimizationHelper == null)
                return;

            didUpdateResultMat = false;

            if (webCamTexture2MatHelper.IsPlaying() && webCamTexture2MatHelper.DidUpdateThisFrame() && !imageOptimizationHelper.IsCurrentFrameSkipped())
            {

                resultMat = webCamTexture2MatHelper.GetMat();
                downScaleResultMat = imageOptimizationHelper.GetDownScaleMat(resultMat);

                didUpdateResultMat = true;
            }
        }

        public override void Dispose()
        {
            if (webCamTexture2MatHelper != null)
                webCamTexture2MatHelper.Dispose();

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
            if (imageOptimizationHelper == null)
                return default;

            return imageOptimizationHelper.downscaleRatio;
        }

        #endregion


        public virtual void Play()
        {
            if (webCamTexture2MatHelper == null)
                return;

            webCamTexture2MatHelper.Play();
        }

        public virtual void Pause()
        {
            if (webCamTexture2MatHelper == null)
                return;

            webCamTexture2MatHelper.Pause();
        }

        public virtual void Stop()
        {
            if (webCamTexture2MatHelper == null)
                return;

            webCamTexture2MatHelper.Stop();
        }

        public virtual void ChangeCamera()
        {
            if (webCamTexture2MatHelper == null)
                return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            string deviceName = webCamTexture2MatHelper.GetDeviceName();
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
                webCamTexture2MatHelper.requestedDeviceName = nextCameraIndex.ToString();
            }
            else
            {
                webCamTexture2MatHelper.requestedIsFrontFacing = !webCamTexture2MatHelper.requestedIsFrontFacing;
            }
#else
            webCamTexture2MatHelper.requestedIsFrontFacing = !webCamTexture2MatHelper.requestedIsFrontFacing;
#endif
        }
    }
}
