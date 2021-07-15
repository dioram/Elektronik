namespace Elektronik.Mesh
{
    public interface IMeshRenderer : ISourceRenderer
    {
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e);
    }
}