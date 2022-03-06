using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace CVVTuber
{
    public class CVVTuberExampleMenuItem : MonoBehaviour
    {
        [MenuItem("Tools/CVVTuberExample/Setup CVVTuberExample", false, 1)]
        public static void SetCVVTuberSettings()
        {
            GameObject cVVTuberModel = GameObject.Find("CVVTuberModel");
            if (cVVTuberModel != null)
            {
                //Undo.RecordObject(cVVTuberModel.transform.localEulerAngles, "Change cVVTuberModel.transform.localEulerAngles");
                //cVVTuberModel.transform.localEulerAngles = new Vector3 (0, 180, 0);

                bool allComplete = true;

                Animator animator = cVVTuberModel.GetComponent<Animator>();

                AnimatorController animCon = animator.runtimeAnimatorController as AnimatorController;
                if (animCon != null)
                {
                    Undo.RecordObject(animCon, "Set true to layer.ikPass");
                    var layers = animCon.layers;
                    bool success = false;
                    foreach (var layer in layers)
                    {
                        if (layer.stateMachine.name == "Base Layer")
                        {
                            layer.iKPass = true;
                            success = true;
                        }
                    }
                    EditorUtility.SetDirty(animCon);

                    if (success)
                    {
                        Debug.Log("Set true to layer.ikPass");
                    }
                    else
                    {
                        Debug.LogError("success == false");
                        allComplete = false;
                    }
                }
                else
                {
                    Debug.LogError("animCon == null");
                    allComplete = false;
                }

                HeadLookAtIKController headLookAtIKController = FindObjectOfType<HeadLookAtIKController>();
                if (headLookAtIKController != null)
                {
                    Undo.RecordObject(headLookAtIKController, "Set animator to headLookAtIKController.target");
                    headLookAtIKController.target = animator;

                    var lookAtLoot = GameObject.Find("LookAtRoot").transform;
                    if (lookAtLoot != null)
                    {
                        headLookAtIKController.lookAtRoot = lookAtLoot;
                        var lookAtTarget = lookAtLoot.transform.Find("LookAtTarget").transform;
                        if (lookAtTarget != null)
                        {
                            headLookAtIKController.lookAtTarget = lookAtTarget;
                        }
                    }
                    EditorUtility.SetDirty(headLookAtIKController);

                    if (headLookAtIKController.lookAtRoot != null && headLookAtIKController.lookAtTarget != null)
                    {
                        Debug.Log("Set animator to headLookAtIKController.target");
                    }
                    else
                    {
                        Debug.LogError("headLookAtIKController.lookAtRoot == null || headLookAtIKController.lookAtTarget == null");
                        allComplete = false;
                    }
                }
                else
                {
                    Debug.LogError("headLookAtIKController == null");
                    allComplete = false;
                }

                HeadRotationController headRotationController = FindObjectOfType<HeadRotationController>();
                Undo.RecordObject(headRotationController, "Set head.transform to headRotationController.target");
                if (headRotationController != null)
                {
                    headRotationController.target = cVVTuberModel.transform.Find("Character001/hips/spine/chest/upper_chest/neck/head").transform;
                    EditorUtility.SetDirty(headRotationController);

                    if (headRotationController.target != null)
                    {
                        Debug.Log("Set head.transform to headRotationController.target");
                    }
                    else
                    {
                        Debug.LogError("headRotationController.target == null");
                        allComplete = false;
                    }
                }
                else
                {
                    Debug.LogError("headRotationController == null");
                    allComplete = false;
                }

                FaceBlendShapeController faceBlendShapeController = FindObjectOfType<FaceBlendShapeController>();
                if (faceBlendShapeController != null)
                {
                    Undo.RecordObject(faceBlendShapeController, "Set SkinnedMeshRenderer to faceBlendShapeController.FACE_DEF");
                    faceBlendShapeController.FACE_DEF = cVVTuberModel.transform.Find("FACE_DEF").GetComponent<SkinnedMeshRenderer>();
                    EditorUtility.SetDirty(faceBlendShapeController);

                    if (faceBlendShapeController.FACE_DEF != null)
                    {
                        Debug.Log("Set SkinnedMeshRenderer to faceBlendShapeController.FACE_DEF");
                    }
                    else
                    {
                        Debug.LogError("faceBlendShapeController.FACE_DEF == null");
                        allComplete = false;
                    }
                }
                else
                {
                    Debug.LogError("faceBlendShapeController == null");
                    allComplete = false;
                }

                if (allComplete)
                    Debug.Log("CVVTuberExample setup is all complete!");

            }
            else
            {
                Debug.LogError("There is no \"CVVTuberModel\" prefab in the scene. Please add \"CVVTuberModel\" prefab to the scene.");
            }
        }
    }
}