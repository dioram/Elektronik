using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;

namespace Elektronik.Clusterization.KMeans
{
    public class KMeansClusterizationFactory : ElektronikPluginsFactoryBase<KMeansSettings>,
                                               IClusterizationAlgorithmFactory
    {
        protected override IElektronikPlugin StartPlugin(KMeansSettings settings, ICSConverter? converter)
        {
            return new KMeansClusterizationAlgorithm(settings, DisplayName);
        }

        public override string DisplayName => "K-means clusterization";
        public override string Description => "Splits point cloud to k clusters using ML.NET";
    }
}