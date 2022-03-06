using UnityEngine;

namespace CVVTuber
{
    public interface IHeadRotationGetter
    {
        Quaternion GetHeadRotation();

        Vector3 GetHeadEulerAngles();
    }
}