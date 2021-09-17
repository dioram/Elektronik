namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface ICloudBlock
    {
        public bool Updated { get; set; }
        GPUItem[] GetItems();
        public float ItemSize { get; set; }
        int ItemsCount { get; set; }
    }
}