using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public class FaceLandmarkHeadPositionAndRotationGetter : CVVTuberProcess, IHeadPositionGetter, IHeadRotationGetter
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

        [SerializeField, InterfaceRestriction(typeof(IFaceLandmarkGetter))]
        protected CVVTuberProcess faceLandmarkGetter;

        protected IFaceLandmarkGetter _faceLandmarkGetterInterface = null;

        protected IFaceLandmarkGetter faceLandmarkGetterInterface
        {
            get
            {
                if (faceLandmarkGetter != null && _faceLandmarkGetterInterface == null)
                    _faceLandmarkGetterInterface = faceLandmarkGetter.GetComponent<IFaceLandmarkGetter>();
                return _faceLandmarkGetterInterface;
            }
        }

        [Header("[Setting]")]

        [Tooltip("Determines if enable low pass filter.")]
        public bool enableLowPassFilter;

        [Tooltip("The position low pass value. (Value in meters)")]
        public float positionLowPass = 2f;

        [Tooltip("The rotation low pass value. (Value in degrees)")]
        public float rotationLowPass = 1f;

        protected PoseData oldPoseData;

        protected Vector3 headPosition;

        protected Quaternion headRotation;

        protected bool didUpdateHeadPositionAndRotation;

        protected float imageWidth = 640;

        protected float imageHeight = 640;

        protected MatOfPoint3f objectPoints68;

        protected MatOfPoint3f objectPoints17;

        protected MatOfPoint3f objectPoints6;

        protected Mat camMatrix;

        protected MatOfDouble distCoeffs;

        protected MatOfPoint2f imagePoints;

        protected Mat rvec;

        protected Mat tvec;

        protected Matrix4x4 invertYM;

        protected Matrix4x4 invertZM;

        protected Matrix4x4 VP;


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Get head rotation from FaceLandmarkGetter.";
        }

        public override void Setup()
        {
            NullCheck(matSourceGetterInterface, "matSourceGetter");
            NullCheck(faceLandmarkGetterInterface, "faceLandmarkGetter");

            //set 3d face object points.
            objectPoints68 = new MatOfPoint3f(
                new Point3(-34, 90, 83),//l eye (Interpupillary breadth)
                new Point3(34, 90, 83),//r eye (Interpupillary breadth)
                new Point3(0.0, 50, 117),//nose (Tip)
                new Point3(0.0, 32, 97),//nose (Subnasale)
                new Point3(-79, 90, 10),//l ear (Bitragion breadth)
                new Point3(79, 90, 10)//r ear (Bitragion breadth)
            );

            objectPoints17 = new MatOfPoint3f(
                new Point3(-34, 90, 83),//l eye (Interpupillary breadth)
                new Point3(34, 90, 83),//r eye (Interpupillary breadth)
                new Point3(0.0, 50, 117),//nose (Tip)
                new Point3(0.0, 32, 97),//nose (Subnasale)
                new Point3(-79, 90, 10),//l ear (Bitragion breadth)
                new Point3(79, 90, 10)//r ear (Bitragion breadth)
            );

            objectPoints6 = new MatOfPoint3f(
                new Point3(-34, 90, 83),//l eye (Interpupillary breadth)
                new Point3(34, 90, 83),//r eye (Interpupillary breadth)
                new Point3(0.0, 50, 117),//nose (Tip)
                new Point3(0.0, 32, 97)//nose (Subnasale)
            );

            imagePoints = new MatOfPoint2f();

            camMatrix = new Mat(3, 3, CvType.CV_64FC1);
            //Debug.Log ("camMatrix " + camMatrix.dump ());

            distCoeffs = new MatOfDouble(0, 0, 0, 0);
            //Debug.Log ("distCoeffs " + distCoeffs.dump ());

            invertYM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
            //Debug.Log ("invertYM " + invertYM.ToString ());

            invertZM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
            //Debug.Log ("invertZM " + invertZM.ToString ());


            didUpdateHeadPositionAndRotation = false;
        }

        public override void UpdateValue()
        {
            if (matSourceGetterInterface == null)
                return;

            if (faceLandmarkGetterInterface == null)
                return;

            Mat rgbaMat = matSourceGetterInterface.GetMatSource();
            if (rgbaMat == null)
            {
                return;
            }
            else
            {
                if (rgbaMat.width() != imageWidth || rgbaMat.height() != imageHeight)
                {
                    imageWidth = rgbaMat.width();
                    imageHeight = rgbaMat.height();
                    SetCameraMatrix(camMatrix, imageWidth, imageHeight);
                }
            }

            didUpdateHeadPositionAndRotation = false;

            List<Vector2> points = faceLandmarkGetterInterface.GetFaceLanmarkPoints();
            if (points != null)
            {
                MatOfPoint3f objectPoints = null;

                if (points.Count == 68)
                {

                    objectPoints = objectPoints68;

                    imagePoints.fromArray(
                        new Point((points[38].x + points[41].x) / 2, (points[38].y + points[41].y) / 2),//l eye (Interpupillary breadth)
                        new Point((points[43].x + points[46].x) / 2, (points[43].y + points[46].y) / 2),//r eye (Interpupillary breadth)
                        new Point(points[30].x, points[30].y),//nose (Tip)
                        new Point(points[33].x, points[33].y),//nose (Subnasale)
                        new Point(points[0].x, points[0].y),//l ear (Bitragion breadth)
                        new Point(points[16].x, points[16].y)//r ear (Bitragion breadth)
                    );

                }
                else if (points.Count == 17)
                {

                    objectPoints = objectPoints17;

                    imagePoints.fromArray(
                        new Point((points[2].x + points[3].x) / 2, (points[2].y + points[3].y) / 2),//l eye (Interpupillary breadth)
                        new Point((points[4].x + points[5].x) / 2, (points[4].y + points[5].y) / 2),//r eye (Interpupillary breadth)
                        new Point(points[0].x, points[0].y),//nose (Tip)
                        new Point(points[1].x, points[1].y),//nose (Subnasale)
                        new Point(points[6].x, points[6].y),//l ear (Bitragion breadth)
                        new Point(points[8].x, points[8].y)//r ear (Bitragion breadth)
                    );

                }
                else if (points.Count == 6)
                {

                    objectPoints = objectPoints6;

                    imagePoints.fromArray(
                        new Point((points[2].x + points[3].x) / 2, (points[2].y + points[3].y) / 2),//l eye (Interpupillary breadth)
                        new Point((points[4].x + points[5].x) / 2, (points[4].y + points[5].y) / 2),//r eye (Interpupillary breadth)
                        new Point(points[0].x, points[0].y),//nose (Tip)
                        new Point(points[1].x, points[1].y)//nose (Subnasale)
                    );
                }

                // Estimate head pose.
                if (rvec == null || tvec == null)
                {
                    rvec = new Mat(3, 1, CvType.CV_64FC1);
                    tvec = new Mat(3, 1, CvType.CV_64FC1);
                    Calib3d.solvePnP(objectPoints, imagePoints, camMatrix, distCoeffs, rvec, tvec);
                }


                double tvec_x = tvec.get(0, 0)[0], tvec_y = tvec.get(1, 0)[0], tvec_z = tvec.get(2, 0)[0];

                bool isNotInViewport = false;
                Vector4 pos = VP * new Vector4((float)tvec_x, (float)tvec_y, (float)tvec_z, 1.0f);
                if (pos.w != 0)
                {
                    float x = pos.x / pos.w, y = pos.y / pos.w, z = pos.z / pos.w;
                    if (x < -1.0f || x > 1.0f || y < -1.0f || y > 1.0f || z < -1.0f || z > 1.0f)
                        isNotInViewport = true;
                }

                if (double.IsNaN(tvec_z) || isNotInViewport)
                { // if tvec is wrong data, do not use extrinsic guesses. (the estimated object is not in the camera field of view)
                    Calib3d.solvePnP(objectPoints, imagePoints, camMatrix, distCoeffs, rvec, tvec);
                }
                else
                {
                    Calib3d.solvePnP(objectPoints, imagePoints, camMatrix, distCoeffs, rvec, tvec, true, Calib3d.SOLVEPNP_ITERATIVE);
                }

                //Debug.Log (tvec.dump () + " " + isNotInViewport);

                if (!isNotInViewport)
                {

                    // Convert to unity pose data.
                    double[] rvecArr = new double[3];
                    rvec.get(0, 0, rvecArr);
                    double[] tvecArr = new double[3];
                    tvec.get(0, 0, tvecArr);
                    PoseData poseData = ARUtils.ConvertRvecTvecToPoseData(rvecArr, tvecArr);

                    // adjust the position to the scale of real-world space.
                    poseData.pos = new Vector3(poseData.pos.x * 0.001f, poseData.pos.y * 0.001f, poseData.pos.z * 0.001f);

                    // Changes in pos/rot below these thresholds are ignored.
                    if (enableLowPassFilter)
                    {
                        ARUtils.LowpassPoseData(ref oldPoseData, ref poseData, positionLowPass, rotationLowPass);
                    }
                    oldPoseData = poseData;

                    Matrix4x4 transformationM = Matrix4x4.TRS(poseData.pos, poseData.rot, Vector3.one);

                    // right-handed coordinates system (OpenCV) to left-handed one (Unity)
                    // https://stackoverflow.com/questions/30234945/change-handedness-of-a-row-major-4x4-transformation-matrix
                    transformationM = invertYM * transformationM * invertYM;

                    // Apply Y-axis and Z-axis refletion matrix. (Adjust the posture of the AR object)
                    transformationM = transformationM * invertYM * invertZM;

                    headPosition = ARUtils.ExtractTranslationFromMatrix(ref transformationM);
                    headRotation = ARUtils.ExtractRotationFromMatrix(ref transformationM);

                    didUpdateHeadPositionAndRotation = true;
                }
            }
        }

        public override void Dispose()
        {
            if (objectPoints68 != null)
                objectPoints68.Dispose();

            if (camMatrix != null)
                camMatrix.Dispose();
            if (distCoeffs != null)
                distCoeffs.Dispose();

            if (imagePoints != null)
                imagePoints.Dispose();

            if (rvec != null)
                rvec.Dispose();

            if (tvec != null)
                tvec.Dispose();
        }

        #endregion


        #region IHeadPositionGetter

        public virtual Vector3 GetHeadPosition()
        {
            if (didUpdateHeadPositionAndRotation)
            {
                return headPosition;
            }
            else
            {
                return Vector3.zero;
            }
        }

        #endregion


        #region IHeadRotationGetter

        public virtual Quaternion GetHeadRotation()
        {
            if (didUpdateHeadPositionAndRotation)
            {
                return headRotation;
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public virtual Vector3 GetHeadEulerAngles()
        {
            if (didUpdateHeadPositionAndRotation)
            {
                return headRotation.eulerAngles;
            }
            else
            {
                return Vector3.zero;
            }
        }

        #endregion


        protected virtual void SetCameraMatrix(Mat camMatrix, float width, float height)
        {
            double max_d = (double)Mathf.Max(width, height);
            double fx = max_d;
            double fy = max_d;
            double cx = width / 2.0;
            double cy = height / 2.0;
            double[] arr = new double[] { fx, 0, cx, 0, fy, cy, 0, 0, 1.0 };
            camMatrix.put(0, 0, arr);

            // create AR camera P * V Matrix
            Matrix4x4 P = ARUtils.CalculateProjectionMatrixFromCameraMatrixValues((float)fx, (float)fy, (float)cx, (float)cy, width, height, 1f, 3000f);
            Matrix4x4 V = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
            VP = P * V;
        }
    }
}