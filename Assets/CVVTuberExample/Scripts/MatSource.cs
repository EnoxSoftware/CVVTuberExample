﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenCVForUnity;

namespace CVVTuber
{

    public class MatSource : CVVTuberProcess
    {

        public virtual Mat GetMatSource ()
        {

            return null;
        }
    }
}
