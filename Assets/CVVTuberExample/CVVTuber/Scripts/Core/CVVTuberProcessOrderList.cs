using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public class CVVTuberProcessOrderList : MonoBehaviour
    {
        [SerializeField]
        List<CVVTuberProcess> processOrderList;

        public List<CVVTuberProcess> GetProcessOrderList ()
        {
            return processOrderList;
        }
    }
}
