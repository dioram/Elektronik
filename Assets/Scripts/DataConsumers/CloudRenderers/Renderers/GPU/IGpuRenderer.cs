namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface IGpuRenderer
    {
        void UpdateDataOnGpu();
        
        void RenderNow();
        
        int RenderQueue { get; }
    }
}