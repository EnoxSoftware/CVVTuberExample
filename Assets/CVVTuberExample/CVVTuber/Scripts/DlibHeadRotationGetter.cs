using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace CVVTuber
{
    public class DlibHeadRotationGetter : CVVTuberProcess
    {
        public MatSourceGetter matSourceGetter;

        public DlibFaceLandmarkGetter dlibFaceLanmarkGetter;

        /// <summary>
        /// Determines if enable low pass filter.
        /// </summary>
        public bool enableLowPassFilter;

        /// <summary>
        /// The position low pass. (Value in meters)
        /// </summary>
        public float positionLowPass = 4f;

        /// <summary>
        /// The rotation low pass. (Value in degrees)
        /// </summary>
        public float rotationLowPass = 2f;

        /// <summary>
        /// The old pose data.
        /// </summary>
        PoseData oldPoseData;

        Quaternion headRotation;

        bool didUpdateHeadRotation;

        float imageWidth = 640;

        float imageHeight = 640;

        /// <summary>
        /// The object points.
        /// </summary>
        MatOfPoint3f objectPoints_68;

        /// <summary>
        /// The cameraparam matrix.
        /// </summary>
        Mat camMatrix;

        /// <summary>
        /// The distortion coeffs.
        /// </summary>
        MatOfDouble distCoeffs;

        /// <summary>
        /// The image points.
        /// </summary>
        MatOfPoint2f imagePoints;

        /// <summary>
        /// The rvec.
        /// </summary>
        Mat rvec;

        /// <summary>
        /// The tvec.
        /// </summary>
        Mat tvec;

        /// <summary>
        /// The matrix that inverts the Y axis.
        /// </summary>
        Matrix4x4 invertYM;

        /// <summary>
        /// The matrix that inverts the Z axis.
        /// </summary>
        Matrix4x4 invertZM;

        public override string GetDescription ()
        {
            return "Get head rotation from DlibFaceLandmarkGetter.";
        }
            
        public override void Setup ()
        {
            //set 3d face object points.
            objectPoints_68 = new MatOfPoint3f (
                new Point3 (-34, 90, 83),//l eye (Interpupillary breadth)
                new Point3 (34, 90, 83),//r eye (Interpupillary breadth)
                new Point3 (0.0, 50, 120),//nose (Nose top)
                new Point3 (-26, 15, 83),//l mouse (Mouth breadth)
                new Point3 (26, 15, 83),//r mouse (Mouth breadth)
                new Point3 (-79, 90, 0.0),//l ear (Bitragion breadth)
                new Point3 (79, 90, 0.0)//r ear (Bitragion breadth)
            );

            imagePoints = new MatOfPoint2f ();

            camMatrix = new Mat (3, 3, CvType.CV_64FC1);
            //Debug.Log ("camMatrix " + camMatrix.dump ());

            distCoeffs = new MatOfDouble (0, 0, 0, 0);
            //Debug.Log ("distCoeffs " + distCoeffs.dump ());

            invertYM = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
            //Debug.Log ("invertYM " + invertYM.ToString ());

            invertZM = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));
            //Debug.Log ("invertZM " + invertZM.ToString ());


            didUpdateHeadRotation = false;
        }
            
        public override void UpdateValue ()
        {
            if (matSourceGetter == null)
                return;

            if (dlibFaceLanmarkGetter == null)
                return;

            Mat rgbaMat = matSourceGetter.GetMatSource ();
            if (rgbaMat == null) {
                return;
            } else {
                if (rgbaMat.width () != imageWidth || rgbaMat.height () != imageHeight) {
                    imageWidth = rgbaMat.width ();
                    imageHeight = rgbaMat.height ();
                    SetCameraMatrix (camMatrix, imageWidth, imageHeight);
                }
            }

            didUpdateHeadRotation = false;

            List<Vector2> points = dlibFaceLanmarkGetter.getFaceLanmarkPoints ();
            if (points != null) {
                MatOfPoint3f objectPoints = null;
                if (points.Count == 68)
                {
                    objectPoints = objectPoints_68;

                    imagePoints.fromArray (
                        new Point ((points [38].x + points [41].x) / 2, (points [38].y + points [41].y) / 2),//l eye (Interpupillary breadth)
                        new Point ((points [43].x + points [46].x) / 2, (points [43].y + points [46].y) / 2),//r eye (Interpupillary breadth)
                        new Point (points [30].x, points [30].y),//nose (Nose top)
                        new Point (points [48].x, points [48].y),//l mouth (Mouth breadth)
                        new Point (points [54].x, points [54].y), //r mouth (Mouth breadth)
                        new Point (points [0].x, points [0].y),//l ear (Bitragion breadth)
                        new Point (points [16].x, points [16].y)//r ear (Bitragion breadth)
                    );
                }

                // Estimate head pose.
                if (rvec == null || tvec == null) {
                    rvec = new Mat (3, 1, CvType.CV_64FC1);
                    tvec = new Mat (3, 1, CvType.CV_64FC1);
                    Calib3d.solvePnP (objectPoints, imagePoints, camMatrix, distCoeffs, rvec, tvec);
                }

                double tvec_z = tvec.get (2, 0) [0];

                if (double.IsNaN (tvec_z) || tvec_z < 0) { // if tvec is wrong data, do not use extrinsic guesses.
                    Calib3d.solvePnP (objectPoints, imagePoints, camMatrix, distCoeffs, rvec, tvec);
                } else {
                    Calib3d.solvePnP (objectPoints, imagePoints, camMatrix, distCoeffs, rvec, tvec, true, Calib3d.SOLVEPNP_ITERATIVE);
                }
                //Debug.Log (tvec.dump());

                if (!double.IsNaN (tvec_z)) {

                    // Convert to unity pose data.
                    double[] rvecArr = new double[3];
                    rvec.get (0, 0, rvecArr);
                    double[] tvecArr = new double[3];
                    tvec.get (0, 0, tvecArr);
                    PoseData poseData = ARUtils.ConvertRvecTvecToPoseData (rvecArr, tvecArr);

                    // Changes in pos/rot below these thresholds are ignored.
                    if (enableLowPassFilter) {
                        ARUtils.LowpassPoseData (ref oldPoseData, ref poseData, positionLowPass, rotationLowPass);
                    }
                    oldPoseData = poseData;


                    Matrix4x4 transformationM = Matrix4x4.TRS (poseData.pos, poseData.rot, Vector3.one);

                    // right-handed coordinates system (OpenCV) to left-handed one (Unity)
                    transformationM = invertYM * transformationM;

                    // Apply Z axis inverted matrix.
                    transformationM = transformationM * invertZM;


                    headRotation = ARUtils.ExtractRotationFromMatrix (ref transformationM);

                    didUpdateHeadRotation = true;
                }
            }
        }

        public override void Dispose ()
        {
            if (objectPoints_68 != null)
                objectPoints_68.Dispose ();

            if (camMatrix != null)
                camMatrix.Dispose ();
            if (distCoeffs != null)
                distCoeffs.Dispose ();

            if (imagePoints != null)
                imagePoints.Dispose ();

            if (rvec != null)
                rvec.Dispose ();

            if (tvec != null)
                tvec.Dispose ();
        }

        public virtual Quaternion GetFaceRotation ()
        {
            if (didUpdateHeadRotation) {
                return headRotation;
            } else {
                return Quaternion.identity;
            }
        }

        public virtual Vector3 GetHeadEulerAngles ()
        {
            if (didUpdateHeadRotation) {
                return headRotation.eulerAngles;
            } else {
                return Vector3.zero;
            }
        }

        private void SetCameraMatrix (Mat camMatrix, float width, float height)
        {
            double max_d = (double)Mathf.Max (width, height);
            double fx = max_d;
            double fy = max_d;
            double cx = width / 2.0;
            double cy = height / 2.0;
            double[] arr = new double[]{ fx, 0, cx, 0, fy, cy, 0, 0, 1.0 };
            camMatrix.put (0, 0, arr);
        }
    }
}