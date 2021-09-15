using Elektronik.Data.PackageObjects;

namespace Elektronik.Clusterization
{
    public interface IClusterable
    {
        public SlamPoint[] GetAllPoints();
    }
}