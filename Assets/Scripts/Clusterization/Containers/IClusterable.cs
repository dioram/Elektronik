using Elektronik.Data.PackageObjects;

namespace Elektronik.Clusterization.Containers
{
    public interface IClusterable
    {
        public SlamPoint[] GetAllPoints();
    }
}