using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Rect = OpenCVForUnity.CoreModule.Rect;

namespace CVVTuber
{
    public class OpenCVFaceRectGetter : CVVTuberProcess, IFaceRectGetter
    {
        [Header("[Input]")]

        [SerializeField, InterfaceRestriction(typeof(IMatSourceGetter))]
        protected CVVTuberProcess matSourceGetter;

        protected IMatSourceGetter _matSourceGetterInterface = null;

        protected IMatSourceGetter matSourceGetterInterface
        {
            get
            {
                if (matSourceGetter != null && _matSourceGetterInterface == null)
                    _matSourceGetterInterface = matSourceGetter.GetComponent<IMatSourceGetter>();
                return _matSourceGetterInterface;
            }
        }

        [Header("[Setting]")]

        public string openCVCascadeFileName;

        public bool useDownScaleMat;

        [Header("[Debug]")]

        public RawImage screen;

        public bool isDebugMode;

        public bool hideImage;

        protected Mat debugMat;

        protected Texture2D debugTexture;

        protected Color32[] debugColors;

        protected UnityEngine.Rect faceRect;

        protected bool didUpdateFaceRect;

        protected Mat grayMat;

        protected CascadeClassifier cascade;

        protected MatOfRect faces;

        protected static readonly string OPENCV_CASCADE_FILENAME_PRESET = "DlibFaceLandmarkDetector/haarcascade_frontalface_alt.xml";

        protected string openCVCascadeFilePath;

#if UNITY_WEBGL
        protected IEnumerator getFilePath_Coroutine;
#endif


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Get face rect from MatSourceGetter.";
        }

        public override void Setup()
        {

            NullCheck(matSourceGetterInterface, "matSourceGetter");

            if (string.IsNullOrEmpty(openCVCascadeFileName))
                openCVCascadeFileName = OPENCV_CASCADE_FILENAME_PRESET;

#if UNITY_WEBGL
            getFilePath_Coroutine = OpenCVForUnity.UnityUtils.Utils.getFilePathAsync(openCVCascadeFileName, (result) =>
            {
                getFilePath_Coroutine = null;

                openCVCascadeFilePath = result;
                Run();
            });
            StartCoroutine(getFilePath_Coroutine);
#else
            openCVCascadeFilePath = OpenCVForUnity.UnityUtils.Utils.getFilePath(openCVCascadeFileName);
            Run();
#endif
        }

        public override void UpdateValue()
        {
            if (cascade == null)
                return;

            if (matSourceGetterInterface == null)
                return;

            didUpdateFaceRect = false;

            Mat rgbaMat = (useDownScaleMat) ? matSourceGetterInterface.GetDownScaleMatSource() : matSourceGetterInterface.GetMatSource();
            if (rgbaMat != null)
            {
                if (isDebugMode && screen != null)
                {

                    if (debugMat != null && (debugMat.width() != rgbaMat.width() || debugMat.height() != rgbaMat.height()))
                    {
                        debugMat.Dispose();
                        debugMat = null;
                    }
                    debugMat = debugMat ?? new Mat(rgbaMat.rows(), rgbaMat.cols(), rgbaMat.type());

                    if (hideImage)
                    {
                        debugMat.setTo(new Scalar(0, 0, 0, 255));
                    }
                    else
                    {
                        rgbaMat.copyTo(debugMat);
                    }

                    if (debugTexture != null && (debugTexture.width != debugMat.width() || debugTexture.height != debugMat.height()))
                    {
                        Texture2D.Destroy(debugTexture);
                        debugTexture = null;
                    }
                    if (debugTexture == null)
                    {
                        debugTexture = new Texture2D(debugMat.width(), debugMat.height(), TextureFormat.RGBA32, false, false);

                        Vector2 size = screen.rectTransform.sizeDelta;
                        screen.rectTransform.sizeDelta = new Vector2(size.x, size.x * (float)debugMat.height() / (float)debugMat.width());
                    }

                    if (debugColors != null && debugColors.Length != debugMat.width() * debugMat.height())
                    {
                        debugColors = new Color32[debugMat.width() * debugMat.height()];
                    }
                    screen.texture = debugTexture;
                    screen.enabled = true;
                }
                else
                {
                    if (screen != null)
                        screen.enabled = false;
                }


                Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
                Imgproc.equalizeHist(grayMat, grayMat);


                if (cascade != null)
                    cascade.detectMultiScale(grayMat, faces, 1.1, 2, 2, // TODO: objdetect.CV_HAAR_SCALE_IMAGE
                        new Size(grayMat.cols() * 0.2, grayMat.rows() * 0.2), new Size());


                Rect[] rects = faces.toArray();
                for (int i = 0; i < rects.Length; i++)
                {
                    if (i == 0)
                    {

                        Rect r = rects[i];
                        if (useDownScaleMat)
                        {
                            // restore to original size rect
                            float downscaleRatio = matSourceGetterInterface.GetDownScaleRatio();
                            faceRect = new UnityEngine.Rect(
                                r.x * downscaleRatio,
                                r.y * downscaleRatio,
                                r.width * downscaleRatio,
                                r.height * downscaleRatio
                            );
                        }
                        else
                        {
                            faceRect = new UnityEngine.Rect(r.x, r.y, r.width, r.height);
                        }

                        didUpdateFaceRect = true;

                        //Debug.Log ("detect faces " + rects [i]);

                        if (isDebugMode && screen != null)
                            Imgproc.rectangle(debugMat, new Point(r.x, r.y), new Point(r.x + r.width, r.y + r.height), new Scalar(255, 0, 0, 255), 2);
                    }
                }

                //Imgproc.putText (debugMat, "W:" + debugMat.width () + " H:" + debugMat.height () + " SO:" + Screen.orientation, new Point (5, debugMat.rows () - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);

                if (isDebugMode && screen != null)
                {
                    OpenCVForUnity.UnityUtils.Utils.matToTexture2D(debugMat, debugTexture, debugColors);
                }
            }
        }

        public override void Dispose()
        {
            if (grayMat != null)
            {
                grayMat.Dispose();
                grayMat = null;
            }

            if (faces != null)
                faces.Dispose();

            if (debugMat != null)
            {
                debugMat.Dispose();
                debugMat = null;
            }

            if (debugTexture != null)
            {
                Texture2D.Destroy(debugTexture);
                debugTexture = null;
            }

#if UNITY_WEBGL
            if (getFilePath_Coroutine != null)
            {
                StopCoroutine(getFilePath_Coroutine);
                ((IDisposable)getFilePath_Coroutine).Dispose();
            }
#endif
        }

        #endregion


        protected virtual void Run()
        {
            cascade = new CascadeClassifier();
            cascade.load(openCVCascadeFilePath);
#if !UNITY_WSA_10_0
            if (cascade.empty())
            {
                Debug.LogError("cascade file is not loaded. Please copy from “DlibFaceLandmarkDetector/StreamingAssets/DlibFaceLandmarkDetector/” to “Assets/StreamingAssets/DlibFaceLandmarkDetector/” folder. ");
            }
#endif

            grayMat = new Mat();
            faces = new MatOfRect();

            didUpdateFaceRect = false;
        }


        #region IFaceRectGetter

        public virtual UnityEngine.Rect GetFaceRect()
        {
            if (didUpdateFaceRect)
            {
                return faceRect;
            }
            else
            {
                return UnityEngine.Rect.zero;
            }
        }

        #endregion
    }
}