using System;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;

namespace Elektronik.Containers
{
    public interface IMeshContainer : ISourceTreeNode, IVisible
    {
        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;

        public void OverrideColors();
    }
}