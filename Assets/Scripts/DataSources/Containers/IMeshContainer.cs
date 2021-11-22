using System;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Interface of container for mesh. </summary>
    public interface IMeshContainer : IVisibleDataSource
    {
        /// <summary> Event that is raised every time mesh updated. </summary>
        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;

        /// <summary> Switches to next render style. </summary>
        public void SwitchShader();
    }
}