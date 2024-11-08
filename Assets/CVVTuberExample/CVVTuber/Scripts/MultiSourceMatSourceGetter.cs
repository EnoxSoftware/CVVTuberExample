using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils.Helper;
using UnityEngine;

namespace CVVTuber
{
    [RequireComponent(typeof(MultiSource2MatHelper), typeof(ImageOptimizationHelper))]
    public class MultiSourceMatSourceGetter : CVVTuberProcess, IMatSourceGetter
    {
        protected MultiSource2MatHelper multiSource2MatHelper;

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
            multiSource2MatHelper = gameObject.GetComponent<MultiSource2MatHelper>();
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper>();

            multiSource2MatHelper.Initialize();

            didUpdateResultMat = false;
        }

        public override void UpdateValue()
        {
            if (multiSource2MatHelper == null)
                return;
            if (imageOptimizationHelper == null)
                return;

            didUpdateResultMat = false;

            if (multiSource2MatHelper.IsPlaying() && multiSource2MatHelper.DidUpdateThisFrame() && !imageOptimizationHelper.IsCurrentFrameSkipped())
            {

                resultMat = multiSource2MatHelper.GetMat();
                downScaleResultMat = imageOptimizationHelper.GetDownScaleMat(resultMat);

                didUpdateResultMat = true;
            }
        }

        public override void Dispose()
        {
            if (multiSource2MatHelper != null)
                multiSource2MatHelper.Dispose();

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
            if (multiSource2MatHelper == null)
                return;

            multiSource2MatHelper.Play();
        }

        public virtual void Pause()
        {
            if (multiSource2MatHelper == null)
                return;

            multiSource2MatHelper.Pause();
        }

        public virtual void Stop()
        {
            if (multiSource2MatHelper == null)
                return;

            multiSource2MatHelper.Stop();
        }

        public virtual void ChangeCamera()
        {
            if (multiSource2MatHelper == null)
                return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            string deviceName = multiSource2MatHelper.GetDeviceName();
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
                multiSource2MatHelper.requestedDeviceName = nextCameraIndex.ToString();
            }
            else
            {
                multiSource2MatHelper.requestedIsFrontFacing = !multiSource2MatHelper.requestedIsFrontFacing;
            }
#else
            multiSource2MatHelper.requestedIsFrontFacing = !multiSource2MatHelper.requestedIsFrontFacing;
#endif
        }
    }
}
