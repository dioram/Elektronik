using Elektronik.Data.PackageObjects;

namespace Elektronik.DataSources.SpecialInterfaces
{
    public interface IClusterable
    {
        public SlamPoint[] GetAllPoints();
    }
}