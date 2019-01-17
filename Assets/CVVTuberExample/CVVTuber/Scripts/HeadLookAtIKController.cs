using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public class HeadLookAtIKController : CVVTuberProcess
    {
        public DlibHeadRotationGetter dlibHeadRotationGetter;

        public Animator target;

        public Transform lookAtRoot;

        public Transform lookAtTarget;

        public Vector3 offsetAngle;

        public bool invertXAxis;

        public bool invertYAxis;

        public bool invertZAxis;

        public bool rotateXAxis;

        public bool rotateYAxis;

        public bool rotateZAxis;

        public bool leapAngle;

        [Range (0, 1)]
        public float leapT = 0.6f;

        protected Vector3 headEulerAngles;
        protected Vector3 oldHeadEulerAngle;

        public override string GetDescription ()
        {
            return "Update head LookAt IK using DlibHeadRotationGetter.";
        }

        public override void Setup ()
        {
            if (target != null) {
                target.gameObject.AddComponent<LookAtIKController> ();

                LookAtIKController headLookAtIKController = target.gameObject.GetComponent<LookAtIKController> ();
                if (lookAtTarget != null) {
                    headLookAtIKController.looktAtTarget = lookAtTarget;
                } else {
                    Debug.LogWarning ("[" + this.GetType ().FullName + "] " + System.Reflection.MethodBase.GetCurrentMethod ().Name + " lookAtTarget == null");
                }
            } else {
                Debug.LogWarning ("[" + this.GetType ().FullName + "] " + System.Reflection.MethodBase.GetCurrentMethod ().Name + " target == null");
                enabled = false;
            }

            if (lookAtRoot != null) {
                oldHeadEulerAngle = lookAtRoot.localEulerAngles;
            } else {
                Debug.LogWarning ("[" + this.GetType ().FullName + "] " + System.Reflection.MethodBase.GetCurrentMethod ().Name + " lookAtRoot == null");
            }
        }

        public override void UpdateValue ()
        {
            if (dlibHeadRotationGetter == null)
                return;
            if (lookAtRoot == null)
                return;

            if (dlibHeadRotationGetter.GetHeadEulerAngles () != Vector3.zero) {
                headEulerAngles = dlibHeadRotationGetter.GetHeadEulerAngles ();

                headEulerAngles = new Vector3 (headEulerAngles.x + offsetAngle.x, headEulerAngles.y + offsetAngle.y, headEulerAngles.z + offsetAngle.z);
                headEulerAngles = new Vector3 (invertXAxis ? -headEulerAngles.x : headEulerAngles.x, invertYAxis ? -headEulerAngles.y : headEulerAngles.y, invertZAxis ? -headEulerAngles.z : headEulerAngles.z);
                headEulerAngles = Quaternion.Euler (rotateXAxis ? 90 : 0, rotateYAxis ? 90 : 0, rotateZAxis ? 90 : 0) * headEulerAngles;
            }

            if (leapAngle) {
                lookAtRoot.localEulerAngles = new Vector3 (Mathf.LerpAngle (oldHeadEulerAngle.x, headEulerAngles.x, leapT), Mathf.LerpAngle (oldHeadEulerAngle.y, headEulerAngles.y, leapT), Mathf.LerpAngle (oldHeadEulerAngle.z, headEulerAngles.z, leapT));
            } else {
                lookAtRoot.localEulerAngles = headEulerAngles;
            }

            oldHeadEulerAngle = lookAtRoot.localEulerAngles;
        }
    }
}