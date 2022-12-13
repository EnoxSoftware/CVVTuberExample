using OpenCVForUnity.CoreModule;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CVVTuberExample
{
    /// <summary>
    /// CV VTuber Example
    /// </summary>
    public class CVVTuberExample : MonoBehaviour
    {
        public Text exampleTitle;
        public Text versionInfo;
        public ScrollRect scrollRect;
        static float verticalNormalizedPosition = 1f;

        public enum DlibShapePredictorNamePreset : int
        {
            sp_human_face_68,
            sp_human_face_68_for_mobile,
            sp_human_face_17,
            sp_human_face_17_for_mobile
        }

        public Dropdown dlibShapePredictorNameDropdown;

        static DlibShapePredictorNamePreset dlibShapePredictorName = DlibShapePredictorNamePreset.sp_human_face_68;

        public static string dlibShapePredictorFileName
        {
            get
            {
                return "DlibFaceLandmarkDetector/" + dlibShapePredictorName.ToString() + ".dat";
            }
        }

        // Use this for initialization
        void Start()
        {
            exampleTitle.text = "CV VTuber Example " + Application.version;

            versionInfo.text = Core.NATIVE_LIBRARY_NAME + " " + OpenCVForUnity.UnityUtils.Utils.getVersion() + " (" + Core.VERSION + ")";
            versionInfo.text += " / dlibfacelandmarkdetector" + " " + DlibFaceLandmarkDetector.UnityUtils.Utils.getVersion();
            versionInfo.text += " / UnityEditor " + Application.unityVersion;
            versionInfo.text += " / ";

#if UNITY_EDITOR
            versionInfo.text += "Editor";
#elif UNITY_STANDALONE_WIN
            versionInfo.text += "Windows";
#elif UNITY_STANDALONE_OSX
            versionInfo.text += "Mac OSX";
#elif UNITY_STANDALONE_LINUX
            versionInfo.text += "Linux";
#elif UNITY_ANDROID
            versionInfo.text += "Android";
#elif UNITY_IOS
            versionInfo.text += "iOS";
#elif UNITY_WSA
            versionInfo.text += "WSA";
#elif UNITY_WEBGL
            versionInfo.text += "WebGL";
#endif
            versionInfo.text += " ";
#if ENABLE_MONO
            versionInfo.text += "Mono";
#elif ENABLE_IL2CPP
            versionInfo.text += "IL2CPP";
#elif ENABLE_DOTNET
            versionInfo.text += ".NET";
#endif

            scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;

            dlibShapePredictorNameDropdown.value = (int)dlibShapePredictorName;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnScrollRectValueChanged()
        {
            verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
        }


        public void OnShowOpenCVLicenseButtonClick()
        {
            SceneManager.LoadScene("ShowOpenCVLicense");
        }

        public void OnWebCamTextureCVVTuberExampleButtonClick()
        {
            SceneManager.LoadScene("WebCamTextureCVVTuberExample");
        }

        public void OnVideoCaptureCVVTuberExampleButtonClick()
        {
            SceneManager.LoadScene("VideoCaptureCVVTuberExample");
        }


        public void OnShowUnityChanLicenseButtonClick()
        {
            SceneManager.LoadScene("ShowUnityChanLicense");
        }

        public void OnUnityChanCVVTuberExampleButtonClick()
        {
            SceneManager.LoadScene("UnityChanCVVTuberExample");
        }

        public void OnLive2DCubism4CVVTuberExampleButtonClick()
        {
            SceneManager.LoadScene("Live2DCubism4CVVTuberExample");
        }

        public void OnVRMCVVTuberExampleButtonClick()
        {
            SceneManager.LoadScene("VRMCVVTuberExample");
        }


        public void OnDlibShapePredictorNameDropdownValueChanged(int result)
        {
            dlibShapePredictorName = (DlibShapePredictorNamePreset)result;
        }
    }
}