using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class UpdateCommand<T> : ICommand
            where T : struct, ICloudItem
    {
        protected readonly ReadOnlyCollection<T> Objs2Restore;
        protected readonly ReadOnlyCollection<T> Objs2Update;

        protected readonly IContainer<T> Container;

        public UpdateCommand(IContainer<T> container, IList<T> objects)
        {
            Container = container;
            Objs2Restore = new ReadOnlyCollection<T>(objects.Select(p => container[p.Id]).ToList());
            Objs2Update = new ReadOnlyCollection<T>(objects.ToList());
        }

        public virtual void Execute() => Container.Update(Objs2Update);
        public virtual void UnExecute() => Container.Update(Objs2Restore);
    }
    
    public class UpdateCommand<TCloudItem, TCloudItemDiff> : ICommand
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : struct, ICloudItemDiff<TCloudItem>
    {
        protected readonly ReadOnlyCollection<TCloudItem> Objs2Restore;
        protected readonly ReadOnlyCollection<TCloudItemDiff> Objs2Update;

        protected readonly IContainer<TCloudItem> Container;

        public UpdateCommand(IContainer<TCloudItem> container, IList<TCloudItemDiff> objects)
        {
            Container = container;
            Objs2Restore = new ReadOnlyCollection<TCloudItem>(objects.Select(p => container[p.Id]).ToList());
            Objs2Update = new ReadOnlyCollection<TCloudItemDiff>(objects.ToList());
        }

        public virtual void Execute() => Container.Update(Objs2Update);
        public virtual void UnExecute() => Container.Update(Objs2Restore);
    }
}