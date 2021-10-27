using System.Collections.Generic;
using Elektronik.DataObjects;

namespace Elektronik.DataSources.Containers
{
    public interface ITrackedCloudContainer<TCloudItem> : ICloudContainer<TCloudItem> where TCloudItem : struct, ICloudItem
    {
        IList<SimpleLine> GetHistory(int id);

        void AddWithHistory(TCloudItem item, IList<SimpleLine> history);
        
        void AddRangeWithHistory(IList<TCloudItem> items, IList<IList<SimpleLine>> histories);
    }
}