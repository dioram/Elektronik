using System;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;

namespace Elektronik.Containers
{
    public interface IMeshContainer : ISourceTree, IVisible
    {
        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;
    }
}