using System.Collections.Generic;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Containers
{
    public interface ITrackedContainer<TCloudItem> : IContainer<TCloudItem> where TCloudItem : ICloudItem
    {
        IList<SimpleLine> GetHistory(int id);

        void AddWithHistory(TCloudItem item, IList<SimpleLine> history);
        
        void AddRangeWithHistory(IEnumerable<TCloudItem> items, IEnumerable<IList<SimpleLine>> histories);
    }
}