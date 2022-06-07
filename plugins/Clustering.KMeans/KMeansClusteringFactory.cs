using Elektronik.PluginsSystem;

namespace Elektronik.Clustering.KMeans
{
    public class KMeansClusterizationFactory : ElektronikPluginsFactoryBase<KMeansSettings>,
                                               IClusteringAlgorithmFactory
    {
        protected override IElektronikPlugin StartPlugin(KMeansSettings settings)
        {
            return new KMeansClusterizationAlgorithm(settings, DisplayName);
        }

        public override string DisplayName => "K-means clusterization";
        public override string Description => "Splits point cloud to k clusters using ML.NET";
    }
}