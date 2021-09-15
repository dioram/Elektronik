using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;

namespace Clusterization.PlanesDetection
{
    public class PlanesDetectionFactory : ElektronikPluginsFactoryBase<PlanesDetectionSettings>,
                                          IClusterizationAlgorithmFactory
    {
        protected override IElektronikPlugin StartPlugin(PlanesDetectionSettings settings, ICSConverter converter)
        {
            return new PlanesDetectionAlgorithm(settings, DisplayName);
        }

        public override string DisplayName => "Planes detection";
        public override string Description => "Splits point cloud to planes";
    }
}