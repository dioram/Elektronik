using System.Collections.Generic;
using Elektronik.Data.PackageObjects;

namespace Elektronik.DataSources.Containers
{
    public interface ITrackedContainer<TCloudItem> : IContainer<TCloudItem> where TCloudItem : struct, ICloudItem
    {
        IList<SimpleLine> GetHistory(int id);

        void AddWithHistory(TCloudItem item, IList<SimpleLine> history);
        
        void AddRangeWithHistory(IList<TCloudItem> items, IList<IList<SimpleLine>> histories);
    }
}