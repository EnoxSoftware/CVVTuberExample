using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{

    //[RequireComponent(typeof(CVVTuberProcessOrderList))]
    public class UnityChanControllManager : MonoBehaviour
    {

        public List<CVVTuberProcess> processOrderList;

        // Use this for initialization
        void Start ()
        {

            //processOrderList = GetComponent<CVVTuberProcessOrderList>().GetProcessOrderList();
            if (processOrderList == null)
                return;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                //Debug.Log("Setup : "+item.gameObject.name);

                item.Setup ();

            }

        }

        // Update is called once per frame
        void Update ()
        {
            if (processOrderList == null)
                return;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                //Debug.Log("UpdateValue : " + item.gameObject.name);

                item.UpdateValue ();
            }
        }

        // Update is called once per frame
        void LateUpdate ()
        {
            if (processOrderList == null)
                return;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                //Debug.Log("LateUpdateValue : " + item.gameObject.name);

                item.LateUpdateValue ();
            }
        }

    }
}

