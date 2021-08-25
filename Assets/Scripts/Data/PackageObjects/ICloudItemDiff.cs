namespace Elektronik.Data.PackageObjects
{
    public interface ICloudItemDiff<TCloudItem> where TCloudItem : ICloudItem
    {
        int Id { get; }
        public TCloudItem Apply();
        public TCloudItem Apply(TCloudItem item);
    }
}