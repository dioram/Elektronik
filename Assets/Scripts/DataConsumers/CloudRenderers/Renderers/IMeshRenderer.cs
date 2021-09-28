using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface IMeshRenderer : IDataConsumer
    {
        public int ShaderId { get; set; }
        
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e);
    }
}