﻿using System.Collections.Generic;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Containers
{
    public interface ITrackedContainer<TCloudItem> : IContainer<TCloudItem>
    {
        IList<SlamLine> GetHistory(int id);

        void AddWithHistory(TCloudItem item, IList<SlamLine> history);
        
        void AddRangeWithHistory(IEnumerable<TCloudItem> items, IEnumerable<IList<SlamLine>> histories);
    }
}