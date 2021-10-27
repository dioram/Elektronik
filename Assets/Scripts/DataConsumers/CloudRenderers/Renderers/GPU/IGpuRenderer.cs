namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Interface for any renderer that works on GPU. </summary>
    public interface IGpuRenderer
    {
        /// <summary> Sends data to GPU. Should be called in MainThread and in MonoBehaviour.Update(). </summary>
        void UpdateDataOnGpu();
        
        /// <summary>
        /// Renders data on GPU. Should be called in MainThread and in MonoBehaviour.OnRenderObject().
        /// </summary>
        void RenderNow();
        
        /// <summary> Order in render queue. More is later. </summary>
        int RenderQueue { get; }
    }
}