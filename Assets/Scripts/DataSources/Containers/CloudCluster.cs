using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    public class CloudCluster : CloudContainer<SlamPoint>, IColorful
    {
        public CloudCluster(Color color, string displayName = "") : base(displayName)
        {
            Color = color;
        }

        public Color Color { get; }
    }
}