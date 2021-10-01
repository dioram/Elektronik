namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface IQueueableRenderer
    {
        void RenderNow();
        
        int RenderQueue { get; }
    }
}