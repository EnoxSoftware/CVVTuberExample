using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace CVVTuber
{
    public class CVVTuberExampleMenuItem : MonoBehaviour
    {
        [MenuItem ("Tools/CVVTuberExample/Setup CVVTuberExample", false, 1)]
        public static void SetCVVTuberSettings ()
        {
            GameObject cVVTuberModel = GameObject.Find ("CVVTuberModel");
            if (cVVTuberModel != null) {
                //Undo.RecordObject(cVVTuberModel.transform.localEulerAngles, "Change cVVTuberModel.transform.localEulerAngles");
                //cVVTuberModel.transform.localEulerAngles = new Vector3 (0, 180, 0);

                Animator animator = cVVTuberModel.GetComponent<Animator> ();

                AnimatorController animCon = animator.runtimeAnimatorController as AnimatorController;
                Undo.RecordObject (animCon, "Set true to layer.ikPass");
                var layers = animCon.layers;
                foreach (var layer in layers) {
                    if (layer.stateMachine.name == "Base Layer") {
                        layer.iKPass = true;
                    }
                }

                HeadLookAtIKController headLookAtIKController = FindObjectOfType<HeadLookAtIKController> ();
                if (headLookAtIKController != null) {
                    Undo.RecordObject (headLookAtIKController, "Set animator to headLookAtIKController.target");
                    headLookAtIKController.target = animator;

                    var lookAtLoot = GameObject.Find ("LookAtRoot").transform;
                    if (lookAtLoot != null) {
                        headLookAtIKController.lookAtRoot = lookAtLoot;
                        var lookAtTarget = lookAtLoot.transform.Find ("LookAtTarget").transform;
                        if (lookAtTarget != null) {
                            headLookAtIKController.lookAtTarget = lookAtTarget;
                        }
                    }
                }

                HeadRotationController headRotationController = FindObjectOfType<HeadRotationController> ();
                Undo.RecordObject (headRotationController, "Set head.transform to headRotationController.target");
                if (headRotationController != null) {
                    headRotationController.target = cVVTuberModel.transform.Find ("Character001/hips/spine/chest/upper_chest/neck/head").transform;
                }

                FaceBlendShapeController faceBlendShapeController = FindObjectOfType<FaceBlendShapeController> ();
                if (faceBlendShapeController != null) {
                    Undo.RecordObject (faceBlendShapeController, "Set SkinnedMeshRenderer to faceBlendShapeController.FACE_DEF");
                    faceBlendShapeController.FACE_DEF = cVVTuberModel.transform.Find ("FACE_DEF").GetComponent<SkinnedMeshRenderer> ();
                }

            } else {
                Debug.LogError ("There is no \"CVVTuberModel\" prefab in the scene. Please add \"CVVTuberModel\" prefab to the scene.");
            }
        }
    }
}