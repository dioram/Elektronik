using Elektronik.Data.PackageObjects;

namespace Elektronik.Plugins.Common.DataDiff
{
    public interface ICloudItemDiff<TCloudItemDiff, TCloudItem> 
            where TCloudItem : ICloudItem
            where TCloudItemDiff: ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        int Id { get; }
        public TCloudItem Apply();
        public TCloudItem Apply(TCloudItem item);

        public TCloudItemDiff Apply(TCloudItemDiff item);
    }
}