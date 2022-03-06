using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVVTuberExample
{
    /// <summary>
    /// Show License
    /// </summary>
    public class ShowOpenCVLicense : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("CVVTuberExample");
        }
    }
}
