using System;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;

namespace Elektronik.Mesh
{
    public interface IMeshContainer : ISourceTree, IVisible
    {
        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;
    }
}