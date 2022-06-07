using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;

namespace Elektronik.Plugins.Common.DataDiff
{

    public static class ConnectableContainerDiffExt
    {
        public static IEnumerable<(int id1, int id2)> GetAllConnections<TCloudItem, TCloudItemDiff>(
            this IConnectableObjectsCloudContainer<TCloudItem> container, TCloudItemDiff diff)
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            return container.GetConnections(diff.Id).Select(c => (diff.Id, c));
        }
    }

    public static class ContainerDiffExt
    {
        public static void Add<TCloudItem, TCloudItemDiff>(this ICloudContainer<TCloudItem> container, TCloudItemDiff diff)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            container.Add(diff.Apply());
        }

        public static void AddRange<TCloudItem, TCloudItemDiff>(this ICloudContainer<TCloudItem> container,
                                                                IEnumerable<TCloudItemDiff> diffs)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            container.AddRange(diffs.Select(d => d.Apply()).ToArray());
        }

        public static void Remove<TCloudItem, TCloudItemDiff>(this ICloudContainer<TCloudItem> container,
                                                              TCloudItemDiff diff)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            container.Remove(diff.Apply());
        }

        public static void Remove<TCloudItem, TCloudItemDiff>(this ICloudContainer<TCloudItem> container,
                                                              IEnumerable<TCloudItemDiff> diffs)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            container.Remove(diffs.Select(d => d.Apply()).ToArray());
        }

        public static void Update<TCloudItem, TCloudItemDiff>(this ICloudContainer<TCloudItem> container,
                                                              TCloudItemDiff diff)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            container.Update(diff.Apply(container[diff.Id]));
        }

        public static void Update<TCloudItem, TCloudItemDiff>(this ICloudContainer<TCloudItem> container,
                                                              IEnumerable<TCloudItemDiff> diff)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            container.Update(diff.Select(d => d.Apply(container[d.Id])).ToArray());
        }
    }
}