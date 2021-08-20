using Elektronik.Containers.EventArgs;

namespace Elektronik.Clouds
{
    public interface IMeshRenderer : ISourceRenderer
    {
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e);
    }
}