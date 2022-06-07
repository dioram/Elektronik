using System;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Interface for data consumers who renders meshes on scene. </summary>
    public interface IMeshRenderer : IDataConsumer, IDisposable
    {
        /// <summary> Selector of mesh shaders. </summary>
        public int ShaderId { get; set; }

        /// <summary> Updates rendering mesh. </summary>
        /// <remarks>
        /// Two objects with same id but from different sender will be handled as different objects.
        /// </remarks>
        /// <param name="sender"> Sender of objects. </param>
        /// <param name="e"> Event argument with list of objects to add. </param>
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e);
    }
}