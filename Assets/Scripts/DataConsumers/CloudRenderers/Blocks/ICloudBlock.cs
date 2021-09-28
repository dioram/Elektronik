namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface ICloudBlock
    {
        public bool Updated { get; set; }
        public float ItemSize { get; set; }
        int ItemsCount { get; set; }
    }
}