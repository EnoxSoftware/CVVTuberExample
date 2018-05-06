using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CVVTuber
{

    public class UnityChanFaceAnimationClipController : CVVTuberProcess
    {

        public Animator animator;

        public AnimationClip[] animations;

        public float delayWeight;

        float current = 0;

        public override string GetDescription ()
        {
            return "Update Face AnimationClip by GUI Button.";
        }

        public override void Setup ()
        {

        }

        public override void LateUpdateValue ()
        {

            if (Input.GetMouseButton (0)) {
                current = 1;
            } else {
                current = Mathf.Lerp (current, 0, delayWeight);
            }
            animator.SetLayerWeight (1, current);
        }

        void OnGUI ()
        {
            foreach (var animation in animations) {
                if (GUILayout.Button (animation.name)) {
                    animator.CrossFade (animation.name, 0);
                }
            }
        }

    }
}
