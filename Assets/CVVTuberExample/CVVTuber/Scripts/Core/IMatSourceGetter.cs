using OpenCVForUnity.CoreModule;

namespace CVVTuber
{
    public interface IMatSourceGetter
    {
        public Mat GetMatSource();

        public Mat GetDownScaleMatSource();

        public float GetDownScaleRatio();
    }
}
