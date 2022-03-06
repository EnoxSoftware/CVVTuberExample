using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    [DisallowMultipleComponent]
    public class CVVTuberProcessOrderList : MonoBehaviour
    {
        [SerializeField]
        List<CVVTuberProcess> processOrderList = default(List<CVVTuberProcess>);

        public List<CVVTuberProcess> GetProcessOrderList()
        {
            return processOrderList;
        }
    }
}
