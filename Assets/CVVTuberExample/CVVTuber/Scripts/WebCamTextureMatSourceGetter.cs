using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.CoreModule;

namespace CVVTuber
{
    [RequireComponent (typeof(WebCamTextureToMatHelper), typeof(ImageOptimizationHelper))]
    public class WebCamTextureMatSourceGetter : MatSourceGetter
    {
        /// <summary>
        /// The webcam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// <summary>
        /// The image optimization helper.
        /// </summary>
        ImageOptimizationHelper imageOptimizationHelper;

        Mat resultMat;

        bool didUpdateResultMat;

        public override string GetDescription ()
        {
            return "Get mat source from WebCamTexture.";
        }

        public override void Setup ()
        {
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper> ();
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper> ();

            #if UNITY_ANDROID && !UNITY_EDITOR
            // Avoids the front camera low light issue that occurs in only some Android devices (e.g. Google Pixel, Pixel2).
            webCamTextureToMatHelper.avoidAndroidFrontCameraLowLightIssue = true;
            #endif
            webCamTextureToMatHelper.Initialize ();

            didUpdateResultMat = false;
        }

        /// <summary>
        /// Raises the web cam texture to mat helper initialized event.
        /// </summary>
        public virtual void OnWebCamTextureToMatHelperInitialized ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperInitialized");

        }

        /// <summary>
        /// Raises the web cam texture to mat helper disposed event.
        /// </summary>
        public virtual void OnWebCamTextureToMatHelperDisposed ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperDisposed");

        }

        /// <summary>
        /// Raises the web cam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public virtual void OnWebCamTextureToMatHelperErrorOccurred (WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log ("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        public override void UpdateValue ()
        {
            didUpdateResultMat = false;

            if (webCamTextureToMatHelper.IsPlaying () && webCamTextureToMatHelper.DidUpdateThisFrame () && !imageOptimizationHelper.IsCurrentFrameSkipped ()) {

                resultMat = imageOptimizationHelper.GetDownScaleMat (webCamTextureToMatHelper.GetMat ());

                didUpdateResultMat = true;
            }
        }

        public override void Dispose ()
        {
            if (webCamTextureToMatHelper != null)
                webCamTextureToMatHelper.Dispose ();

            if (imageOptimizationHelper != null)
                imageOptimizationHelper.Dispose ();

            if (resultMat != null) {
                resultMat.Dispose ();
                resultMat = null;
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

        public virtual void ChangeCamera ()
        {
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.IsFrontFacing ();
        }
    }
}
