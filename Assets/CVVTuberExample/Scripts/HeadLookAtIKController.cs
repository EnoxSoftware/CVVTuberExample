using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CVVTuber
{

    public class HeadLookAtIKController : CVVTuberProcess
    {

        public DlibHeadRotationGetter dlibHeadRotationGetter;

        public Animator animator;

        public Transform lookAtRoot;

        public Transform lookAtTarget;


        public bool rotateXAxis;
        public bool rotateYAxis;
        public bool rotateZAxis;

        public Vector3 offsetAngle;

        public bool leapAngle;

        [Range (0, 1)]
        public float leapT = 0.6f;

        Vector3 headEulerAngles;
        Vector3 oldHeadEulerAngle;

        public override string GetDescription ()
        {
            return "Update Head LookAt IK using DlibHeadRotationGetter.";
        }

        public override void Setup ()
        {
            animator.gameObject.AddComponent<LookAtIKController> ();

            LookAtIKController headLookAtIKController = animator.gameObject.GetComponent<LookAtIKController> ();
            headLookAtIKController.looktAtTarget = lookAtTarget;

            oldHeadEulerAngle = lookAtRoot.localEulerAngles;
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
