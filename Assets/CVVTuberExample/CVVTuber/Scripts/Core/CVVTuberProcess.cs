using UnityEngine;

namespace CVVTuber
{
    public abstract class CVVTuberProcess : MonoBehaviour
    {
        public bool callInUnityLifeCycle;

        // Use this for initialization
        protected virtual void Start()
        {
            if (callInUnityLifeCycle)
                Setup();
        }

        protected virtual void FixedUpdate()
        {
            if (callInUnityLifeCycle)
                FixedUpdateValue();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (callInUnityLifeCycle)
                UpdateValue();
        }

        // Update is called once per frame
        protected virtual void LateUpdate()
        {
            if (callInUnityLifeCycle)
                LateUpdateValue();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        public virtual void Setup()
        {

        }

        public virtual void FixedUpdateValue()
        {

        }

        public virtual void UpdateValue()
        {

        }

        public virtual void LateUpdateValue()
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual string GetDescription()
        {
            return "";
        }

        protected virtual void NullCheck(System.Object obj, string name)
        {
            if (obj == null)
                NullWarning(name);
        }

        protected virtual void NullWarning(string name)
        {
            Debug.LogWarning("[" + this.GetType().FullName + "] " + name + " == null");
        }
    }
}