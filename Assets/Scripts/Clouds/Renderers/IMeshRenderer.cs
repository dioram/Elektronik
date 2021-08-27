using Elektronik.Containers.EventArgs;

namespace Elektronik.Clouds
{
    public interface IMeshRenderer : ISourceRenderer
    {
        public bool OverrideColors { get; set; }
        
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e);
    }
}