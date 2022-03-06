using CVVTuber;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVVTuberExample
{
    public class VideoCaptureCVVTuberExample : MonoBehaviour
    {
        /// <summary>
        /// The video capture mat source getter.
        /// </summary>
        public VideoCaptureMatSourceGetter videoCaptureMatSourceGetter;

        /// <summary>
        /// The dlib face landmark getter.
        /// </summary>
        public DlibFaceLandmarkGetter dlibFaceLandmarkGetter;

        // Use this for initialization
        void Start()
        {
            // Load global settings.
            dlibFaceLandmarkGetter.dlibShapePredictorFileName = CVVTuberExample.dlibShapePredictorFileName;
            dlibFaceLandmarkGetter.dlibShapePredictorMobileFileName = CVVTuberExample.dlibShapePredictorFileName;
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("CVVTuberExample");
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick()
        {
            videoCaptureMatSourceGetter.Play();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick()
        {
            videoCaptureMatSourceGetter.Stop();
        }
    }
}