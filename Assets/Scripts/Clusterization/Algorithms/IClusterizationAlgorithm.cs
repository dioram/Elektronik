using System.Collections.Generic;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Clusterization.Algorithms
{
    public interface IClusterizationAlgorithm
    {
        public List<List<SlamPoint>> Compute(IList<SlamPoint> items);
    }
}