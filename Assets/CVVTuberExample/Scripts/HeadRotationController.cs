using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CVVTuber
{
    public class HeadRotationController : CVVTuberProcess
    {
        public DlibHeadRotationGetter dlibHeadRotationGetter;

        public Transform target;

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
            return "Update Head rotation of VRM using DlibHeadRotationGetter.";
        }

        public override void Setup ()
        {
            oldHeadEulerAngle = target.localEulerAngles;
        }

        public override void LateUpdateValue ()
        {
            if (dlibHeadRotationGetter == null)
                return;
            if (target == null)
                return;


            if (dlibHeadRotationGetter.GetHeadEulerAngles () != Vector3.zero) {
                headEulerAngles = dlibHeadRotationGetter.GetHeadEulerAngles ();


                headEulerAngles = new Vector3 (headEulerAngles.x + offsetAngle.x, headEulerAngles.y + offsetAngle.y, headEulerAngles.z + offsetAngle.z);

                headEulerAngles = Quaternion.Euler (rotateXAxis ? 90 : 0, rotateYAxis ? 90 : 0, rotateZAxis ? 90 : 0) * headEulerAngles;
            }
            
            if (leapAngle) {

                target.localEulerAngles = new Vector3 (Mathf.LerpAngle (oldHeadEulerAngle.x, headEulerAngles.x, leapT), Mathf.LerpAngle (oldHeadEulerAngle.y, headEulerAngles.y, leapT), Mathf.LerpAngle (oldHeadEulerAngle.z, headEulerAngles.z, leapT));

            } else {
                target.localEulerAngles = headEulerAngles;
            }

            oldHeadEulerAngle = target.localEulerAngles;

        }
    }
}
