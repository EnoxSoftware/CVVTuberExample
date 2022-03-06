using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public interface IFaceLandmarkGetter
    {
        List<Vector2> GetFaceLanmarkPoints();
    }
}