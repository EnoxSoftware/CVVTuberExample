using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenCVForUnity;
using DlibFaceLandmarkDetectorExample;

namespace CVVTuber
{
    
    [RequireComponent (typeof(WebCamTextureToMatHelper))]
    public class WebCamTextureMatSourceGetter : MatSource
    {

        /// <summary>
        /// The webcam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        Mat resultMat;

        bool didUpdateResultMat;

        #if UNITY_ANDROID && !UNITY_EDITOR
        float rearCameraRequestedFPS;
#endif


        public override string GetDescription ()
        {
            return "Get MatSource from WebCamTexture.";
        }

        
        public override void Setup ()
        {
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper> ();

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
        public void OnWebCamTextureToMatHelperInitialized ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperInitialized");

        }

        /// <summary>
        /// Raises the web cam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperDisposed");

        }

        /// <summary>
        /// Raises the web cam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred (WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log ("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        
        public override void UpdateValue ()
        {
            didUpdateResultMat = false;

            if (webCamTextureToMatHelper.IsPlaying () && webCamTextureToMatHelper.DidUpdateThisFrame ()) {
                //Debug.Log("getSourceMat() ");

                resultMat = webCamTextureToMatHelper.GetMat ();

                didUpdateResultMat = true;
            }

        }

        public override void Dispose ()
        {
            if (webCamTextureToMatHelper != null)
                webCamTextureToMatHelper.Dispose ();

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
    }
}
