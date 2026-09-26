using UnityEngine;

namespace CVVTuber
{
    public interface IHeadRotationGetter
    {
        public Quaternion GetHeadRotation();

        public Vector3 GetHeadEulerAngles();
    }
}
