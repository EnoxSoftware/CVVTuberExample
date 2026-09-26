using UnityEngine;

namespace CVVTuber
{
    public class AnimatorLookAtIKController : MonoBehaviour
    {
        public Animator animator;

        public Transform looktAtTarget;

        [Range(0, 1.0f)]
        public float weight = 1.0f;

        [Range(0, 1.0f)]
        public float bodyWeight = 0.5f;

        [Range(0, 1.0f)]
        public float headWeight = 0.0f;

        [Range(0, 1.0f)]
        public float eyesWeightt = 0.5f;

        [Range(0, 1.0f)]
        public float clampWeight = 0.0f;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (animator != null)
            {
                animator.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeightt, clampWeight);
                animator.SetLookAtPosition(looktAtTarget.position);
            }
        }
    }
}
