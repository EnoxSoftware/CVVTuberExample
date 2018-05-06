using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using OpenCVForUnity;
using DlibFaceLandmarkDetector;

using DlibFaceLandmarkDetectorExample;


namespace CVVTuber
{

    public class DlibHeadRotationGetter : CVVTuberProcess
    {

        public DlibFaceLandmarkGetter dlibFaceLanmarkGetter;

        private Quaternion headRotation;

        bool didUpdateHeadRotation;

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

        /// <summary>
        /// The 3d face object points.
        /// </summary>
        MatOfPoint3f objectPoints68;

        /// <summary>
        /// The 3d face object points.
        /// </summary>
        MatOfPoint3f objectPoints5;

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

        /// <summary>
        /// The transformation matrix.
        /// </summary>
        Matrix4x4 transformationM = new Matrix4x4 ();

        /// <summary>
        /// The transformation matrix for AR.
        /// </summary>
        Matrix4x4 ARM;



        public override string GetDescription ()
        {
            return "Get HeadRotation from DlibFaceLandmarkGetter.";
        }

        // Use this for initialization
        public override void Setup ()
        {
            //set 3d face object points.
            objectPoints68 = new MatOfPoint3f (
                new Point3 (-34, 90, 83),//l eye (Interpupillary breadth)
                new Point3 (34, 90, 83),//r eye (Interpupillary breadth)
                new Point3 (0.0, 50, 120),//nose (Nose top)
                new Point3 (-26, 15, 83),//l mouse (Mouth breadth)
                new Point3 (26, 15, 83),//r mouse (Mouth breadth)
                new Point3 (-79, 90, 0.0),//l ear (Bitragion breadth)
                new Point3 (79, 90, 0.0)//r ear (Bitragion breadth)
            );
            objectPoints5 = new MatOfPoint3f (
                new Point3 (-23, 90, 83),//l eye (Inner corner of the eye)
                new Point3 (23, 90, 83),//r eye (Inner corner of the eye)
                new Point3 (-50, 90, 80),//l eye (Tail of the eye)
                new Point3 (50, 90, 80),//r eye (Tail of the eye)
                new Point3 (0.0, 50, 120)//nose (Nose top)
            );
            imagePoints = new MatOfPoint2f ();


            float width = 640;
            float height = 480;


            //set cameraparam
            int max_d = (int)Mathf.Max (width, height);
            double fx = max_d;
            double fy = max_d;
            double cx = width / 2.0f;
            double cy = height / 2.0f;
            camMatrix = new Mat (3, 3, CvType.CV_64FC1);
            camMatrix.put (0, 0, fx);
            camMatrix.put (0, 1, 0);
            camMatrix.put (0, 2, cx);
            camMatrix.put (1, 0, 0);
            camMatrix.put (1, 1, fy);
            camMatrix.put (1, 2, cy);
            camMatrix.put (2, 0, 0);
            camMatrix.put (2, 1, 0);
            camMatrix.put (2, 2, 1.0f);
            Debug.Log ("camMatrix " + camMatrix.dump ());


            distCoeffs = new MatOfDouble (0, 0, 0, 0);
            Debug.Log ("distCoeffs " + distCoeffs.dump ());


            invertYM = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
            Debug.Log ("invertYM " + invertYM.ToString ());

            invertZM = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));
            Debug.Log ("invertZM " + invertZM.ToString ());


            didUpdateHeadRotation = false;
        }

        // Update is called once per frame
        public override void UpdateValue ()
        {
            

            if (dlibFaceLanmarkGetter == null)
                return;

            didUpdateHeadRotation = false;


            List<Vector2> points = dlibFaceLanmarkGetter.getFaceLanmarkPoints ();
            if (points != null) {
                MatOfPoint3f objectPoints = null;
                if (points.Count == 68) {

                    objectPoints = objectPoints68;

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


                    transformationM = Matrix4x4.TRS (poseData.pos, poseData.rot, Vector3.one);

                    // right-handed coordinates system (OpenCV) to left-handed one (Unity)
                    ARM = invertYM * transformationM;

                    // Apply Z axis inverted matrix.
                    ARM = ARM * invertZM;



                    headRotation = ARUtils.ExtractRotationFromMatrix (ref ARM);

                    didUpdateHeadRotation = true;

                }
            }
        }

        public override void Dispose ()
        {
            if (objectPoints68 != null)
                objectPoints68.Dispose ();


            if (objectPoints5 != null)
                objectPoints5.Dispose ();

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


        public Quaternion GetFaceRotation ()
        {
            if (didUpdateHeadRotation) {
                return headRotation;
            } else {
                return Quaternion.identity;
            }

        }

        public Vector3 GetHeadEulerAngles ()
        {
            if (didUpdateHeadRotation) {
                return headRotation.eulerAngles;
            } else {
                return Vector3.zero;
            }
        }

    }
}
