using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenCVForUnity;

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

        #if UNITY_ANDROID && !UNITY_EDITOR
        float rearCameraRequestedFPS;
        #endif

        public override string GetDescription ()
        {
            return "Get mat source from WebCamTexture.";
        }
        
        public override void Setup ()
        {
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper> ();
            imageOptimizationHelper = gameObject.GetComponent<ImageOptimizationHelper> ();

            #if UNITY_ANDROID && !UNITY_EDITOR
            // Set the requestedFPS parameter to avoid the problem of the WebCamTexture image becoming low light on some Android devices. (Pixel, pixel 2)
            // https://forum.unity.com/threads/android-webcamtexture-in-low-light-only-some-models.520656/
            // https://forum.unity.com/threads/released-opencv-for-unity.277080/page-33#post-3445178
            rearCameraRequestedFPS = webCamTextureToMatHelper.requestedFPS;
            if (webCamTextureToMatHelper.requestedIsFrontFacing) {                
                webCamTextureToMatHelper.requestedFPS = 15;
                webCamTextureToMatHelper.Initialize ();
            } else {
                webCamTextureToMatHelper.Initialize ();
            }
            #else
            webCamTextureToMatHelper.Initialize ();
            #endif

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
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (!webCamTextureToMatHelper.IsFrontFacing ()) {
                rearCameraRequestedFPS = webCamTextureToMatHelper.requestedFPS;
                webCamTextureToMatHelper.Initialize (!webCamTextureToMatHelper.IsFrontFacing (), 15, webCamTextureToMatHelper.rotate90Degree);
            } else {                
                webCamTextureToMatHelper.Initialize (!webCamTextureToMatHelper.IsFrontFacing (), rearCameraRequestedFPS, webCamTextureToMatHelper.rotate90Degree);
            }
            #else
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.IsFrontFacing ();
            #endif
        }
    }
}
