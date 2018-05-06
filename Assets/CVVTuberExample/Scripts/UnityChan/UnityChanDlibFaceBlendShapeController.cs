using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


using OpenCVForUnity;
using DlibFaceLandmarkDetector;

using DlibFaceLandmarkDetectorExample;

namespace CVVTuber
{
    public class UnityChanDlibFaceBlendShapeController : CVVTuberProcess
    {

        /// <summary>
        /// DlibFaceLandmarkGetter
        /// </summary>
        public DlibFaceLandmarkGetter dlibFaceLandmarkGetter;

        public bool enableEye;

        public bool enableBrow;

        public bool enableMouth;

        public SkinnedMeshRenderer EYE_DEF;
        public SkinnedMeshRenderer EL_DEF;
        public SkinnedMeshRenderer BLW_DEF;
        public SkinnedMeshRenderer MTH_DEF;



        [Range (0, 100)]
        public float EyeParam = 0;

        [Range (0, 100)]
        public float BrowParam = 0;

        [Range (0, 100)]
        public float MouthParam = 0;

        [Range (0, 1)]
        public float eyeLeapT = 0.6f;

        [Range (0, 1)]
        public float browLeapT = 0.6f;

        [Range (0, 1)]
        public float mouthLeapT = 0.5f;

        List<Vector2> oldPoints;




        public override string GetDescription ()
        {
            return "Update Face BlendShape using DlibFaceLandmarkGetter.";
        }

        public override void Setup ()
        {

        }


        public override void LateUpdateValue ()
        {
            if (dlibFaceLandmarkGetter == null)
                return;
            //if (target == null) return;


            List<Vector2> points = dlibFaceLandmarkGetter.getFaceLanmarkPoints ();

            if (points != null) {
                FaceBlendShapeUpdate (points);

                oldPoints = points;
            } else {
                if (oldPoints != null) {
                    FaceBlendShapeUpdate (oldPoints);
                }
            }

        }


        private void FaceBlendShapeUpdate (List<Vector2> points)
        {
            if (enableEye) {
                float eyeOpen = (getLeftEyeOpenRatio (points) + getRightEyeOpenRatio (points)) / 2.0f;
                //Debug.Log("eyeOpen " + eyeOpen);

//                if (eyeOpen >= 0.5f) {
//                    eyeOpen = 1.0f;
//                } else if (eyeOpen >= 0.2f) {
//                    eyeOpen = 0.5f;
//                } else {
//                    eyeOpen = 0.0f;
//                }
                if (eyeOpen >= 0.3f) {
                    eyeOpen = 1.0f;
                } else {
                    eyeOpen = 0.0f;
                }
                EyeParam = Mathf.Lerp (EyeParam, 100 - (eyeOpen * 100), eyeLeapT);

                EYE_DEF.SetBlendShapeWeight (6, EyeParam);
                EL_DEF.SetBlendShapeWeight (6, EyeParam);
            }

            if (enableBrow) {
                float browOpen = (getLeftBrowOpenRatio (points) + getRightBrowOpenRatio (points)) / 2.0f;
                //Debug.Log("browOpen " + browOpen);

                if (browOpen >= 0.7f) {
                    browOpen = 1.0f;
                } else if (browOpen >= 0.3f) {
                    browOpen = 0.5f;
                } else {
                    browOpen = 0.0f;
                }
                BrowParam = Mathf.Lerp (BrowParam, browOpen * 100, browLeapT);

                BLW_DEF.SetBlendShapeWeight (0, BrowParam);
            }


            if (enableMouth) {
                float mouthOpen = getMouthOpenYRatio (points);
                //Debug.Log("mouthOpen " + mouthOpen);

                if (mouthOpen >= 0.7f) {
                    mouthOpen = 1.0f;
                } else if (mouthOpen >= 0.25f) {
                    mouthOpen = 0.5f;
                } else {
                    mouthOpen = 0.0f;
                }
                MouthParam = Mathf.Lerp (MouthParam, mouthOpen * 100, mouthLeapT);

                MTH_DEF.SetBlendShapeWeight (0, MouthParam);

            }


        }

        private float getLeftEyeOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [44].y - points [46].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw leftEyeSize " + size);

            return Mathf.InverseLerp (0.1f, 0.2f, size);
        }

        private float getRightEyeOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [37].y - points [41].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw rightEyeSize " + size);

            return Mathf.InverseLerp (0.1f, 0.2f, size);
        }

        private float getLeftBrowOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [24].y - points [27].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw leftBrowSize " + size);

            return Mathf.InverseLerp (0.5f, 0.7f, size);
        }

        private float getRightBrowOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [19].y - points [27].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw rightBrowSize " + size);

            return Mathf.InverseLerp (0.5f, 0.7f, size);
        }

        private float getMouthOpenYRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [62].y - points [66].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw mouthYSize " + size);

            return Mathf.InverseLerp (0.0f, 0.5f, size);
        }

        private float getMouthOpenXRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [60].y - points [64].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw mouthXSize " + size);

            return Mathf.InverseLerp (0.05f, 0.1f, size);
        }

    }
}
