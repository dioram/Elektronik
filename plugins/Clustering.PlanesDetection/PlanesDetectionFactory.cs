﻿using Elektronik.PluginsSystem;

namespace Elektronik.Clustering.PlanesDetection
{
    public class PlanesDetectionFactory : ElektronikPluginsFactoryBase<PlanesDetectionSettings>,
                                          IClusteringAlgorithmFactory
    {
        protected override IElektronikPlugin StartPlugin(PlanesDetectionSettings settings)
        {
            return new PlanesDetectionAlgorithm(settings, DisplayName);
        }

        public override string DisplayName => "Planes detection";
        public override string Description => "Splits point cloud to planes";
    }
}