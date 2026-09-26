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
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
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
