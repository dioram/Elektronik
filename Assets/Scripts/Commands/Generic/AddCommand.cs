using System.Collections.Generic;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;

namespace Elektronik.Commands.Generic
{
    public class AddCommand<T> : ICommand where T : struct, ICloudItem
    {
        protected readonly IList<T> AddedObjects;
        protected readonly IContainer<T> Container;
        
        public AddCommand(IContainer<T> container, IList<T> objects)
        {
            Container = container;
            AddedObjects = objects;
        }        

        public virtual void Execute() => Container.AddRange(AddedObjects);
        public virtual void UnExecute() => Container.Remove(AddedObjects);
    }
    
    public class AddCommand<TCloudItem, TCloudItemDiff> : ICommand 
            where TCloudItem : struct, ICloudItem 
            where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        protected readonly IList<TCloudItemDiff> AddedObjects;
        protected readonly IContainer<TCloudItem> Container;
        
        public AddCommand(IContainer<TCloudItem> container, IList<TCloudItemDiff> objects)
        {
            Container = container;
            AddedObjects = objects;
        }        

        public virtual void Execute() => Container.AddRange(AddedObjects);
        public virtual void UnExecute() => Container.Remove(AddedObjects);
    }
}
