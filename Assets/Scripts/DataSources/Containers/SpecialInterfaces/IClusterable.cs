using Elektronik.Data.PackageObjects;

namespace Elektronik.DataSources.Containers.SpecialInterfaces
{
    public interface IClusterable
    {
        public SlamPoint[] GetAllPoints();
    }
}