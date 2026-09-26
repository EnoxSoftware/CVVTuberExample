using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public interface IFaceLandmarkGetter
    {
        public List<Vector2> GetFaceLanmarkPoints();
    }
}
