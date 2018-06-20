using UnityEngine;
using System.Collections;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace CVVTuberExample
{
    public class ShowOpenCVLicense : MonoBehaviour
    {
        // Use this for initialization
        void Start ()
        {
    
        }
    
        // Update is called once per frame
        void Update ()
        {
    
        }

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
    }
}
