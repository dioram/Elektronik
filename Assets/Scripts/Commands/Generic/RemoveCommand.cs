using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class RemoveCommand<T> : ICommand 
        where T: struct, ICloudItem
    {
        protected readonly ReadOnlyCollection<T> Objs2Remove;
        private readonly IContainer<T> _container;

        public RemoveCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            _container = container;
            Objs2Remove = new ReadOnlyCollection<T>(objects.Select(p => _container[p.Id]).ToList());
        }

        public virtual void Execute() => _container.Remove(Objs2Remove);
        public virtual void UnExecute() => _container.AddRange(Objs2Remove);
    }
    public class RemoveCommand<TCloudItem, TCloudItemDiff> : ICommand 
            where TCloudItem: struct, ICloudItem
            where TCloudItemDiff: struct, ICloudItemDiff<TCloudItem>
    {
        protected readonly ReadOnlyCollection<TCloudItem> Objs2Remove;
        private readonly IContainer<TCloudItem> _container;

        public RemoveCommand(IContainer<TCloudItem> container, IEnumerable<TCloudItemDiff> objects)
        {
            _container = container;
            Objs2Remove = new ReadOnlyCollection<TCloudItem>(objects.Select(p => _container[p.Id]).ToList());
        }

        public virtual void Execute() => _container.Remove(Objs2Remove);
        public virtual void UnExecute() => _container.AddRange(Objs2Remove);
    }
}
