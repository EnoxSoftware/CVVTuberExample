using UnityEngine;

namespace CVVTuber
{
    public class HeadRotationController : CVVTuberProcess
    {
        [Header("[Input]")]

        [SerializeField, InterfaceRestriction(typeof(IHeadRotationGetter))]
        protected CVVTuberProcess headRotationGetter;

        protected IHeadRotationGetter _headRotationGetterInterface = null;

        protected IHeadRotationGetter headRotationGetterInterface
        {
            get
            {
                if (headRotationGetter != null && _headRotationGetterInterface == null)
                    _headRotationGetterInterface = headRotationGetter.GetComponent<IHeadRotationGetter>();
                return _headRotationGetterInterface;
            }
        }

        [Header("[Setting]")]

        public Vector3 offsetAngle;

        public bool invertXAxis;

        public bool invertYAxis;

        public bool invertZAxis;

        public bool rotateXAxis;

        public bool rotateYAxis;

        public bool rotateZAxis;

        public bool leapAngle;

        [Range(0, 1)]
        public float leapT = 0.6f;

        [Header("[Target]")]

        public Transform target;

        protected Vector3 headEulerAngles;

        protected Vector3 oldHeadEulerAngle;


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Update head rotation of target transform using HeadRotationGetter.";
        }

        public override void Setup()
        {
            NullCheck(headRotationGetterInterface, "headRotationGetter");

            if (target != null)
            {
                oldHeadEulerAngle = target.localEulerAngles;
            }
            else
            {
                NullWarning("target");
            }
        }

        public override void LateUpdateValue()
        {
            if (headRotationGetterInterface == null)
                return;
            if (target == null)
                return;

            if (headRotationGetterInterface.GetHeadEulerAngles() != Vector3.zero)
            {
                headEulerAngles = headRotationGetterInterface.GetHeadEulerAngles();

                headEulerAngles = new Vector3(headEulerAngles.x + offsetAngle.x, headEulerAngles.y + offsetAngle.y, headEulerAngles.z + offsetAngle.z);
                headEulerAngles = new Vector3(invertXAxis ? -headEulerAngles.x : headEulerAngles.x, invertYAxis ? -headEulerAngles.y : headEulerAngles.y, invertZAxis ? -headEulerAngles.z : headEulerAngles.z);
                headEulerAngles = Quaternion.Euler(rotateXAxis ? 90 : 0, rotateYAxis ? 90 : 0, rotateZAxis ? 90 : 0) * headEulerAngles;
            }

            if (leapAngle)
            {
                target.localEulerAngles = new Vector3(Mathf.LerpAngle(oldHeadEulerAngle.x, headEulerAngles.x, leapT), Mathf.LerpAngle(oldHeadEulerAngle.y, headEulerAngles.y, leapT), Mathf.LerpAngle(oldHeadEulerAngle.z, headEulerAngles.z, leapT));
            }
            else
            {
                target.localEulerAngles = headEulerAngles;
            }

            oldHeadEulerAngle = target.localEulerAngles;
        }

        #endregion
    }
}