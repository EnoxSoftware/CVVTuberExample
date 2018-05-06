using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace CVVTuber
{
    public class CVVTuberMenuItem : MonoBehaviour
    {
        [MenuItem ("Tools/CVVTuberExample/Setup UnityChanCVVTuberExample", false, 1)]
        static void SetPluginImportSettings ()
        {

            GameObject unitychan = GameObject.Find ("unitychan");
            if (unitychan != null) {
                unitychan.transform.localEulerAngles = new Vector3 (0, 180, 0);

                Animator animator = unitychan.GetComponent<Animator> ();

                AnimatorController animCon = animator.runtimeAnimatorController as AnimatorController;
                var layers = animCon.layers;
                foreach (var layer in layers) {
                    if (layer.stateMachine.name == "Base Layer") {
                        layer.iKPass = true;
                    }
                }
                EditorUtility.SetDirty (animator);

                IdleChanger idleChanger = unitychan.GetComponent<IdleChanger> ();
                idleChanger.enabled = false;

                FaceUpdate faceUpdate = unitychan.GetComponent<FaceUpdate> ();
                faceUpdate.enabled = false;

                HeadLookAtIKController headLookAtIKController = FindObjectOfType<HeadLookAtIKController> ();
                if (headLookAtIKController != null) {
                    headLookAtIKController.animator = animator;

                    EditorUtility.SetDirty (headLookAtIKController);
                }

                HeadRotationController headRotationController = FindObjectOfType<HeadRotationController> ();
                if (headRotationController != null) {
                    headRotationController.target = GameObject.Find ("Character1_Head").transform;

                    EditorUtility.SetDirty (headRotationController);
                }

                UnityChanDlibFaceBlendShapeController unityChanDlibFaceBlendShapeController = FindObjectOfType<UnityChanDlibFaceBlendShapeController> ();
                if (unityChanDlibFaceBlendShapeController != null) {
                    unityChanDlibFaceBlendShapeController.EYE_DEF = GameObject.Find ("EYE_DEF").GetComponent<SkinnedMeshRenderer> ();
                    unityChanDlibFaceBlendShapeController.EL_DEF = GameObject.Find ("EL_DEF").GetComponent<SkinnedMeshRenderer> ();
                    unityChanDlibFaceBlendShapeController.BLW_DEF = GameObject.Find ("BLW_DEF").GetComponent<SkinnedMeshRenderer> ();
                    unityChanDlibFaceBlendShapeController.MTH_DEF = GameObject.Find ("MTH_DEF").GetComponent<SkinnedMeshRenderer> ();


                    EditorUtility.SetDirty (unityChanDlibFaceBlendShapeController);
                }

                UnityChanFaceAnimationClipController unityChanFaceAnimationClipController = FindObjectOfType<UnityChanFaceAnimationClipController> ();
                if (unityChanFaceAnimationClipController != null) {
                    unityChanFaceAnimationClipController.animator = animator;

                    EditorUtility.SetDirty (unityChanFaceAnimationClipController);
                }

                EditorUtility.SetDirty (unitychan);
            } else {
                Debug.LogError ("There is no \"unitychan\" prefab in the scene. Please add \"unitychan\" prefab to the scene.");
            }

            
        }
    }
}
