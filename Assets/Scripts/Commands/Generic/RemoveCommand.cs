using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class RemoveCommand<T> : ICommand
            where T : struct, ICloudItem
    {
        protected readonly IList<T> Objs2Remove;
        private readonly IContainer<T> _container;

        public RemoveCommand(IContainer<T> container, IList<T> objects)
        {
            _container = container;
            Objs2Remove = objects;
        }

        public virtual void Execute() => _container.Remove(Objs2Remove);
        public virtual void UnExecute() => _container.AddRange(Objs2Remove);
    }

    public class RemoveCommand<TCloudItem, TCloudItemDiff> : ICommand
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        protected readonly IList<TCloudItemDiff> Objs2Remove;
        protected TCloudItem[] Objs2Add;
        private readonly IContainer<TCloudItem> _container;

        public RemoveCommand(IContainer<TCloudItem> container, IList<TCloudItemDiff> objects)
        {
            _container = container;
            Objs2Remove = objects;
        }

        public virtual void Execute()
        {
            Objs2Add = _container.Remove(Objs2Remove.Select(o => o.Id).ToArray()).ToArray();
        }

        public virtual void UnExecute() => _container.AddRange(Objs2Add);
    }
}