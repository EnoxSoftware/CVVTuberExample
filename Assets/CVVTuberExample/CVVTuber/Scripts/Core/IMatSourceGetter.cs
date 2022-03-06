using OpenCVForUnity.CoreModule;

namespace CVVTuber
{
    public interface IMatSourceGetter
    {
        Mat GetMatSource();

        Mat GetDownScaleMatSource();

        float GetDownScaleRatio();
    }
}