using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Extensions.SourceToMat;
using OpenCVForUnity.UnityIntegration.Helper.Optimization;
using OpenCVForUnity.UnityIntegration.Helper.SourceToMat;
using UnityEngine;

namespace CVVTuber
{
    [RequireComponent(typeof(MultiSourceToMatHelper), typeof(ImageOptimizationHelper))]
    public class MultiSourceMatSourceGetter : CVVTuberProcess, IMatSourceGetter
    {
        protected MultiSourceToMatHelper multiSourceToMatHelper;

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
            multiSourceToMatHelper = gameObject.GetComponent<MultiSourceToMatHelper>();
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper>();

            RegisterSourceEvents();
            multiSourceToMatHelper.Initialize();

            didUpdateResultMat = false;
        }

        public override void UpdateValue()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

            if (imageOptimizationHelper == null)
            {
                return;
            }

            if (!multiSourceToMatHelper.IsInitialized)
            {
                return;
            }

            didUpdateResultMat = false;

            if (multiSourceToMatHelper.IsPlaying && multiSourceToMatHelper.DidUpdateThisFrame && !imageOptimizationHelper.IsCurrentFrameSkipped())
            {

                resultMat = multiSourceToMatHelper.FrameMat;
                downScaleResultMat = imageOptimizationHelper.GetDownScaleMat(resultMat);

                didUpdateResultMat = true;
            }
        }

        public override void Dispose()
        {
            if (multiSourceToMatHelper != null)
            {
                UnregisterSourceEvents();
                multiSourceToMatHelper.Dispose();
            }

            if (imageOptimizationHelper != null)
            {
                imageOptimizationHelper.Dispose();
            }

            if (resultMat != null)
            {
                resultMat = null;
            }

            didUpdateResultMat = false;
            downScaleResultMat = null;
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
            {
                return default;
            }

            return imageOptimizationHelper.DownscaleRatio;
        }

        #endregion

        public virtual void Play()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

            multiSourceToMatHelper.Play();
        }

        public virtual void Pause()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

            multiSourceToMatHelper.Pause();
        }

        public virtual void Stop()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

            multiSourceToMatHelper.Stop();
        }

        public virtual void ChangeCamera()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            ICameraMatSource cameraMatSource = multiSourceToMatHelper.MatSource as ICameraMatSource;
            ICameraToMatHelperControls cameraControls = multiSourceToMatHelper.ActiveHelper as ICameraToMatHelperControls;
            ICameraFacingToMatHelperControls facingControls = multiSourceToMatHelper.ActiveHelper as ICameraFacingToMatHelperControls;
            if (cameraMatSource == null || cameraControls == null || facingControls == null)
            {
                return;
            }

            string deviceName = cameraMatSource.DeviceName;
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
                cameraControls.RequestedDeviceName = nextCameraIndex.ToString();
            }
            else
            {
                facingControls.RequestedIsFrontFacing = !facingControls.RequestedIsFrontFacing;
            }
#else
            ICameraFacingToMatHelperControls facingControls = multiSourceToMatHelper.ActiveHelper as ICameraFacingToMatHelperControls;
            if (facingControls == null)
            {
                return;
            }

            facingControls.RequestedIsFrontFacing = !facingControls.RequestedIsFrontFacing;
#endif
        }

        private void RegisterSourceEvents()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

            multiSourceToMatHelper.OnInitialized.RemoveListener(OnSourceInitialized);
            multiSourceToMatHelper.OnFrameMatLayoutChanged.RemoveListener(OnFrameMatLayoutChanged);
            multiSourceToMatHelper.OnReleased.RemoveListener(OnSourceReleased);
            multiSourceToMatHelper.OnDisposed.RemoveListener(OnSourceDisposed);

            multiSourceToMatHelper.OnInitialized.AddListener(OnSourceInitialized);
            multiSourceToMatHelper.OnFrameMatLayoutChanged.AddListener(OnFrameMatLayoutChanged);
            multiSourceToMatHelper.OnReleased.AddListener(OnSourceReleased);
            multiSourceToMatHelper.OnDisposed.AddListener(OnSourceDisposed);
        }

        private void UnregisterSourceEvents()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

            multiSourceToMatHelper.OnInitialized.RemoveListener(OnSourceInitialized);
            multiSourceToMatHelper.OnFrameMatLayoutChanged.RemoveListener(OnFrameMatLayoutChanged);
            multiSourceToMatHelper.OnReleased.RemoveListener(OnSourceReleased);
            multiSourceToMatHelper.OnDisposed.RemoveListener(OnSourceDisposed);
        }

        private void OnSourceInitialized()
        {
            if (multiSourceToMatHelper == null)
            {
                return;
            }

            if (!multiSourceToMatHelper.IsPlaying && !multiSourceToMatHelper.IsPaused)
            {
                multiSourceToMatHelper.Play();
            }
        }

        private void OnFrameMatLayoutChanged()
        {
            ClearFrameState();
        }

        private void OnSourceReleased()
        {
            ClearFrameState();
        }

        private void OnSourceDisposed()
        {
            ClearFrameState();
        }

        private void ClearFrameState()
        {
            didUpdateResultMat = false;
            resultMat = null;
            downScaleResultMat = null;
        }
    }
}
