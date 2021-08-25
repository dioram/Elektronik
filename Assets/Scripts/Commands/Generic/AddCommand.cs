using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class AddCommand<T> : ICommand where T : ICloudItem
    {
        protected readonly ReadOnlyCollection<T> AddedObjects;
        protected readonly IContainer<T> Container;
        
        public AddCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            Container = container;
            AddedObjects = new ReadOnlyCollection<T>(objects.Where(p => !container.Contains(p.Id)).ToList());
        }        

        public virtual void Execute() => Container.AddRange(AddedObjects);
        public virtual void UnExecute() => Container.Remove(AddedObjects);
    }
    
    public class AddCommand<TCloudItem, TCloudItemDiff> : ICommand 
            where TCloudItem : ICloudItem 
            where TCloudItemDiff : ICloudItemDiff<TCloudItem>
    {
        protected readonly ReadOnlyCollection<TCloudItemDiff> AddedObjects;
        protected readonly IContainer<TCloudItem> Container;
        
        public AddCommand(IContainer<TCloudItem> container, IEnumerable<TCloudItemDiff> objects)
        {
            Container = container;
            AddedObjects = new ReadOnlyCollection<TCloudItemDiff>(objects.ToList());
        }        

        public virtual void Execute() => Container.AddRange(AddedObjects);
        public virtual void UnExecute() => Container.Remove(AddedObjects);
    }
}
