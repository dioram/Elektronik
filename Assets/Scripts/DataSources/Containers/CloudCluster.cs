using Elektronik.DataObjects;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Container for cluster of cloud points. </summary>
    public class CloudCluster : CloudContainer<SlamPoint>, IColorfulDataSource
    {
        /// <summary> Color of this cluster. </summary>
        /// <param name="color"> Color of this cluster. </param>
        /// <param name="displayName"> Name of this cluster. </param>
        public CloudCluster(Color color, string displayName = "") : base(displayName)
        {
            Color = color;
        }

        /// <inheritdoc />
        public Color Color { get; }
    }
}