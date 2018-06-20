using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace CVVTuber
{
    public class DlibFaceBlendShapeController : CVVTuberProcess
    {
        public DlibFaceLandmarkGetter dlibFaceLandmarkGetter;

        public SkinnedMeshRenderer FACE_DEF;

        public bool enableEye;

        public bool enableBrow;

        public bool enableMouth;

        [Range (0, 1)]
        public float EyeParam = 0;

        [Range (0, 1)]
        public float BrowParam = 0;

        [Range (0, 1)]
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
            return "Update face BlendShape using DlibFaceLandmarkGetter.";
        }

        public override void Setup ()
        {

        }

        public override void LateUpdateValue ()
        {
            if (dlibFaceLandmarkGetter == null)
                return;

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

        protected virtual void FaceBlendShapeUpdate (List<Vector2> points)
        {
            if (enableEye) {
                float eyeOpen = (getLeftEyeOpenRatio (points) + getRightEyeOpenRatio (points)) / 2.0f;
                //Debug.Log("eyeOpen " + eyeOpen);

                if (eyeOpen >= 0.4f) {
                    eyeOpen = 1.0f;
                } else {
                    eyeOpen = 0.0f;
                }
                EyeParam = Mathf.Lerp (EyeParam, 1 - eyeOpen, eyeLeapT);

                FACE_DEF.SetBlendShapeWeight (0, EyeParam * 100);
                FACE_DEF.SetBlendShapeWeight (1, EyeParam * 100);
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
                MouthParam = Mathf.Lerp (MouthParam, mouthOpen, mouthLeapT);

                if (MouthParam >= 0.7f) {
                    FACE_DEF.SetBlendShapeWeight (2, MouthParam * 100);
                } else if (MouthParam >= 0.25f) {
                    FACE_DEF.SetBlendShapeWeight (2, MouthParam * 80);
                } else {
                    FACE_DEF.SetBlendShapeWeight (2, 0);
                }
            }
        }

        protected virtual float getLeftEyeOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [44].y - points [46].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw leftEyeSize " + size);

            return Mathf.InverseLerp (0.1f, 0.16f, size);
        }

        protected virtual float getRightEyeOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [37].y - points [41].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw rightEyeSize " + size);

            return Mathf.InverseLerp (0.1f, 0.16f, size);
        }

        protected virtual float getLeftBrowOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [24].y - points [27].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw leftBrowSize " + size);

            return Mathf.InverseLerp (0.5f, 0.7f, size);
        }

        protected virtual float getRightBrowOpenRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [19].y - points [27].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw rightBrowSize " + size);

            return Mathf.InverseLerp (0.5f, 0.7f, size);
        }

        protected virtual float getMouthOpenYRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points [62].y - points [66].y) / Mathf.Abs (points [27].y - points [30].y);

            //Debug.Log("raw mouthYSize " + size);

            return Mathf.InverseLerp (0.0f, 0.5f, size);
        }

        protected virtual float getMouthOpenXRatio (List<Vector2> points)
        {
            float size = Mathf.Abs (points[48].x - points[54].x) / (Mathf.Abs(points[31].x - points[35].x));

            //Debug.Log("raw mouthXSize " + size);

            return Mathf.InverseLerp (1.8f, 2.0f, size);
        }
    }
}