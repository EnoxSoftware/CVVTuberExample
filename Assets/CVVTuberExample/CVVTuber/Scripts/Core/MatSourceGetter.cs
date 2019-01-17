using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;

namespace CVVTuber
{
    public class MatSourceGetter : CVVTuberProcess
    {
        public virtual Mat GetMatSource ()
        {
            return null;
        }
    }
}
