using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public abstract class FaceAnimationController : CVVTuberProcess
    {
        [Header("[Input]")]

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

        public bool enableBrow;

        public bool enableEye;

        public bool enableMouth;

        [Range(0, 1)]
        public float BrowParam = 0;

        [Range(0, 1)]
        public float EyeParam = 0;

        [Range(0, 1)]
        public float MouthOpenParam = 0;

        [Range(0, 1)]
        public float MouthSizeParam = 0;

        [Range(0, 1)]
        public float browLeapT = 0.6f;

        [Range(0, 1)]
        public float eyeLeapT = 0.6f;

        [Range(0, 1)]
        public float mouthLeapT = 0.6f;

        protected List<Vector2> oldPoints;

        protected float distanceOfLeftEyeHeight;

        protected float distanceOfRightEyeHeight;

        protected float distanceOfNoseHeight;

        protected float distanceBetweenLeftPupliAndEyebrow;

        protected float distanceBetweenRightPupliAndEyebrow;

        protected float distanceOfMouthHeight;

        protected float distanceOfMouthWidth;

        protected float distanceBetweenEyes;


        #region CVVTuberProcess

        public override void Setup()
        {
            NullCheck(faceLandmarkGetterInterface, "faceLandmarkGetter");
        }

        public override void UpdateValue()
        {
            if (faceLandmarkGetterInterface == null)
                return;

            List<Vector2> points = faceLandmarkGetterInterface.GetFaceLanmarkPoints();

            if (points != null)
            {
                CalculateFacePartsDistance(points);
                UpdateFaceAnimation(points);

                oldPoints = points;
            }
            else
            {
                if (oldPoints != null)
                {
                    UpdateFaceAnimation(oldPoints);
                }
            }
        }

        #endregion


        protected virtual void CalculateFacePartsDistance(List<Vector2> points)
        {
            if (points.Count == 68)
            {
                distanceOfLeftEyeHeight = new Vector2((points[47].x + points[46].x) / 2 - (points[43].x + points[44].x) / 2, (points[47].y + points[46].y) / 2 - (points[43].y + points[44].y) / 2).sqrMagnitude;
                distanceOfRightEyeHeight = new Vector2((points[40].x + points[41].x) / 2 - (points[38].x + points[37].x) / 2, (points[40].y + points[41].y) / 2 - (points[38].y + points[37].y) / 2).sqrMagnitude;
                distanceOfNoseHeight = new Vector2(points[33].x - (points[39].x + points[42].x) / 2, points[33].y - (points[39].y + points[42].y) / 2).sqrMagnitude;
                distanceBetweenLeftPupliAndEyebrow = new Vector2(points[24].x - (points[42].x + points[45].x) / 2, points[24].y - (points[42].y + points[45].y) / 2).sqrMagnitude;
                distanceBetweenRightPupliAndEyebrow = new Vector2(points[19].x - (points[39].x + points[36].x) / 2, points[19].y - (points[39].y + points[36].y) / 2).sqrMagnitude;
                distanceOfMouthHeight = new Vector2(points[51].x - points[57].x, points[51].y - points[57].y).sqrMagnitude;
                distanceOfMouthWidth = new Vector2(points[48].x - points[54].x, points[48].y - points[54].y).sqrMagnitude;
                distanceBetweenEyes = new Vector2(points[39].x - points[42].x, points[39].y - points[42].y).sqrMagnitude;

            }
            else if (points.Count == 17)
            {
                distanceOfLeftEyeHeight = new Vector2(points[12].x - points[11].x, points[12].y - points[11].y).sqrMagnitude;
                distanceOfRightEyeHeight = new Vector2(points[10].x - points[9].x, points[10].y - points[9].y).sqrMagnitude;
                distanceOfNoseHeight = new Vector2(points[1].x - (points[3].x + points[4].x) / 2, points[1].y - (points[3].y + points[4].y) / 2).sqrMagnitude;
                distanceBetweenLeftPupliAndEyebrow = 0;
                distanceBetweenRightPupliAndEyebrow = 0;
                distanceOfMouthHeight = new Vector2(points[14].x - points[16].x, points[14].y - points[16].y).sqrMagnitude;
                distanceOfMouthWidth = new Vector2(points[13].x - points[15].x, points[13].y - points[15].y).sqrMagnitude;
                distanceBetweenEyes = new Vector2(points[3].x - points[4].x, points[3].y - points[4].y).sqrMagnitude;
            }
        }

        protected abstract void UpdateFaceAnimation(List<Vector2> points);

        protected virtual float GetLeftEyeOpenRatio(List<Vector2> points)
        {
            float ratio = distanceOfLeftEyeHeight / distanceOfNoseHeight;
            //Debug.Log ("raw LeftEyeOpen ratio: " + ratio);
            return Mathf.InverseLerp(0.003f, 0.009f, ratio);
        }

        protected virtual float GetRightEyeOpenRatio(List<Vector2> points)
        {
            float ratio = distanceOfRightEyeHeight / distanceOfNoseHeight;
            //Debug.Log ("raw RightEyeOpen ratio: " + ratio);
            return Mathf.InverseLerp(0.003f, 0.009f, ratio);
        }

        protected virtual float GetLeftEyebrowUPRatio(List<Vector2> points)
        {
            float ratio = distanceBetweenLeftPupliAndEyebrow / distanceOfNoseHeight;
            //Debug.Log ("raw LeftEyebrowUP ratio: " + ratio);
            return Mathf.InverseLerp(0.18f, 0.48f, ratio);
        }

        protected virtual float GetRightEyebrowUPRatio(List<Vector2> points)
        {
            float ratio = distanceBetweenRightPupliAndEyebrow / distanceOfNoseHeight;
            //Debug.Log ("raw RightEyebrowUP ratio: " + ratio);
            return Mathf.InverseLerp(0.18f, 0.48f, ratio);
        }

        protected virtual float GetMouthOpenYRatio(List<Vector2> points)
        {
            float ratio = distanceOfMouthHeight / distanceOfNoseHeight;
            //Debug.Log ("raw MouthOpenY ratio: " + ratio);
            return Mathf.InverseLerp(0.06f, 0.6f, ratio);
        }

        protected virtual float GetMouthOpenXRatio(List<Vector2> points)
        {
            float ratio = distanceOfMouthWidth / distanceBetweenEyes;
            //Debug.Log ("raw MouthOpenX ratio: " + ratio);
            return Mathf.InverseLerp(1.8f, 2.0f, ratio);
        }
    }
}