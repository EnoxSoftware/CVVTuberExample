using CVVTuber;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVVTuberExample
{
    public class MultiSourceCVVTuberExample : MonoBehaviour
    {
        /// <summary>
        /// The multi source mat source getter.
        /// </summary>
        public MultiSourceMatSourceGetter multiSourceMatSourceGetter;

        /// <summary>
        /// The dlib face landmark getter.
        /// </summary>
        public DlibFaceLandmarkGetter dlibFaceLandmarkGetter;

        // Use this for initialization
        void Start()
        {
            // Load global settings.
            dlibFaceLandmarkGetter.dlibShapePredictorFilePath = CVVTuberExample.dlibShapePredictorFilePath;
            dlibFaceLandmarkGetter.dlibShapePredictorMobileFilePath = CVVTuberExample.dlibShapePredictorFilePath;
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("CVVTuberExample");
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick()
        {
            multiSourceMatSourceGetter.ChangeCamera();
        }
    }
}