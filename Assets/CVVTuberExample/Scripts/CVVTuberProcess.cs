using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{

    public class CVVTuberProcess : MonoBehaviour
    {


        public bool callInUnityLifeCycle;

        // Use this for initialization
        void Start ()
        {
            if (callInUnityLifeCycle)
                Setup ();
        }

        void FixedUpdate ()
        {
            if (callInUnityLifeCycle)
                FixedUpdateValue ();
        }

        // Update is called once per frame
        void Update ()
        {
            if (callInUnityLifeCycle)
                UpdateValue ();
        }

        // Update is called once per frame
        void LateUpdate ()
        {
            if (callInUnityLifeCycle)
                LateUpdateValue ();
        }

        void OnDestroy ()
        {
            //if (callInUnityLifeCycle) Dispose();
            Dispose ();
        }

        public virtual void Setup ()
        {

        }

        public virtual void FixedUpdateValue ()
        {

        }

        public virtual void UpdateValue ()
        {

        }

        public virtual void LateUpdateValue ()
        {

        }

        public virtual void Dispose ()
        {

        }

        public virtual string GetDescription ()
        {
            return "";
        }
    }
}
