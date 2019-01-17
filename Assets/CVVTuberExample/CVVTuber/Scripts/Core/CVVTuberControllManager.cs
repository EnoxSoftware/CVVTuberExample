using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    [RequireComponent (typeof(CVVTuberProcessOrderList))]
    public class CVVTuberControllManager : MonoBehaviour
    {
        protected List<CVVTuberProcess> processOrderList;

        // Use this for initialization
        protected virtual IEnumerator Start ()
        {
            enabled = false;

            processOrderList = GetComponent<CVVTuberProcessOrderList> ().GetProcessOrderList ();
            if (processOrderList == null)
                yield break;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                //Debug.Log("Setup : "+item.gameObject.name);

                item.Setup ();
            }

            enabled = true;
        }

        // Update is called once per frame
        protected virtual void Update ()
        {
            if (processOrderList == null)
                return;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                if (!item.gameObject.activeInHierarchy || !item.enabled)
                    continue;

                //Debug.Log("UpdateValue : " + item.gameObject.name);

                item.UpdateValue ();
            }
        }

        // Update is called once per frame
        protected virtual void LateUpdate ()
        {
            if (processOrderList == null)
                return;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                if (!item.gameObject.activeInHierarchy || !item.enabled)
                    continue;

                //Debug.Log("LateUpdateValue : " + item.gameObject.name);

                item.LateUpdateValue ();
            }
        }

        protected virtual void OnDestroy ()
        {
            Dispose ();
        }

        public virtual void Dispose ()
        {

        }
    }
}