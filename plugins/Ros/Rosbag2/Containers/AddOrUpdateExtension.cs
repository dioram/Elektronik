using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Ros.Rosbag2.Containers
{
    public static class AddOrUpdateExtension
    {
        public static void AddOrUpdate<TCloudItem>(this IContainer<TCloudItem> container, TCloudItem item)
                where TCloudItem : struct, ICloudItem
        {
            if (container.Contains(item)) container.Update(item);
            else container.Add(item);
        }

        public static void AddOrUpdate<TCloudItem>(this IContainer<TCloudItem> container, IEnumerable<TCloudItem> items)
                where TCloudItem : struct, ICloudItem
        {
            var added = new List<TCloudItem>();
            var updated = new List<TCloudItem>();

            foreach (var item in items)
            {
                if (container.Contains(item)) updated.Add(item);
                else added.Add(item);
            }

            container.AddRange(added);
            container.Update(updated);
        }
    }
}