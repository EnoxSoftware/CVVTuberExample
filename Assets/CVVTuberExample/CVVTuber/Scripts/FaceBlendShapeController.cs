using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public class FaceBlendShapeController : FaceAnimationController
    {
        [Header("[Target]")]

        public SkinnedMeshRenderer FACE_DEF;


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Update face BlendShape using FaceLandmarkGetter.";
        }

        public override void LateUpdateValue()
        {
            if (FACE_DEF == null)
                return;

            if (enableEye)
            {
                FACE_DEF.SetBlendShapeWeight(0, EyeParam * 100);
                FACE_DEF.SetBlendShapeWeight(1, EyeParam * 100);
            }

            if (enableMouth)
            {
                if (MouthOpenParam >= 0.7f)
                {
                    FACE_DEF.SetBlendShapeWeight(2, MouthOpenParam * 100);
                }
                else if (MouthOpenParam >= 0.25f)
                {
                    FACE_DEF.SetBlendShapeWeight(2, MouthOpenParam * 80);
                }
                else
                {
                    FACE_DEF.SetBlendShapeWeight(2, 0);
                }
            }
        }

        #endregion


        #region FaceAnimationController

        public override void Setup()
        {
            base.Setup();

            NullCheck(FACE_DEF, "FACE_DEF");
        }

        protected override void UpdateFaceAnimation(List<Vector2> points)
        {
            if (enableEye)
            {
                float eyeOpen = (GetLeftEyeOpenRatio(points) + GetRightEyeOpenRatio(points)) / 2.0f;
                //Debug.Log("eyeOpen " + eyeOpen);

                if (eyeOpen >= 0.4f)
                {
                    eyeOpen = 1.0f;
                }
                else
                {
                    eyeOpen = 0.0f;
                }
                EyeParam = Mathf.Lerp(EyeParam, 1 - eyeOpen, eyeLeapT);
            }

            if (enableMouth)
            {
                float mouthOpen = GetMouthOpenYRatio(points);
                //Debug.Log("mouthOpen " + mouthOpen);

                if (mouthOpen >= 0.7f)
                {
                    mouthOpen = 1.0f;
                }
                else if (mouthOpen >= 0.25f)
                {
                    mouthOpen = 0.5f;
                }
                else
                {
                    mouthOpen = 0.0f;
                }
                MouthOpenParam = Mathf.Lerp(MouthOpenParam, mouthOpen, mouthLeapT);
            }
        }

        #endregion
    }
}