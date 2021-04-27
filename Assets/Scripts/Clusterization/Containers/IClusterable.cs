using System.Collections.Generic;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Clusterization.Containers
{
    public interface IClusterable
    {
        public IEnumerable<SlamPoint> GetAllPoints();
    }
}