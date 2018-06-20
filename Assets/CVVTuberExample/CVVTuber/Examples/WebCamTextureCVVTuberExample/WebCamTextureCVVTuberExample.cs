using UnityEngine;
using System.Collections;
using CVVTuber;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace CVVTuberExample
{
    public class WebCamTextureCVVTuberExample : MonoBehaviour
    {
        /// <summary>
        /// The webcam texture mat source getter.
        /// </summary>
        public WebCamTextureMatSourceGetter webCamTextureMatSourceGetter;

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick ()
        {
            #if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene ("CVVTuberExample");
            #else
            Application.LoadLevel ("CVVTuberExample");
            #endif
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick ()
        {
            webCamTextureMatSourceGetter.ChangeCamera ();
        }
    }
}