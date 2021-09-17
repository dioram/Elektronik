using System;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.Containers.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    public interface IMeshContainer : ISourceTreeNode, IVisible
    {
        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;

        public void OverrideColors();
    }
}