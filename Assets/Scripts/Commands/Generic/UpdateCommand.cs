using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class UpdateCommand<T> : ICommand
            where T : struct, ICloudItem
    {
        protected readonly IContainer<T> Container;
        protected readonly T[] Objs2Update;
        protected T[] Objs2Restore;

        public UpdateCommand(IContainer<T> container, T[] objects)
        {
            Container = container;
            Objs2Update = objects;
        }

        public virtual void Execute()
        {
            Objs2Restore = Objs2Update.Select(p => Container[p.Id]).ToArray();
            Container.Update(Objs2Update);
        }

        public virtual void UnExecute() => Container.Update(Objs2Restore);
    }
    
    public class UpdateCommand<TCloudItem, TCloudItemDiff> : ICommand
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        protected readonly IContainer<TCloudItem> Container;
        protected readonly TCloudItemDiff[] Objs2Update;
        protected TCloudItem[] Objs2Restore;

        public UpdateCommand(IContainer<TCloudItem> container, TCloudItemDiff[] objects)
        {
            Container = container;
            Objs2Update = objects;
        }

        public virtual void Execute()
        {
            Objs2Restore = Objs2Update.Select(p => Container[p.Id]).ToArray();
            Container.Update(Objs2Update);
        }

        public virtual void UnExecute() => Container.Update(Objs2Restore);
    }
}