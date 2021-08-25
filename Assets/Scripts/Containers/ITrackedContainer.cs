using System.Collections.Generic;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Containers
{
    public interface ITrackedContainer<TCloudItem> : IContainer<TCloudItem> where TCloudItem : ICloudItem
    {
        IList<SlamLine> GetHistory(int id);

        void AddWithHistory(TCloudItem item, IList<SlamLine> history);
        
        void AddRangeWithHistory(IEnumerable<TCloudItem> items, IEnumerable<IList<SlamLine>> histories);
    }
}