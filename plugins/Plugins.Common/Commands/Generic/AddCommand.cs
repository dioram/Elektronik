using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.DataDiff;

namespace Elektronik.Plugins.Common.Commands.Generic
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

        protected bool Equals(AddCommand<T> other)
        {
            return AddedObjects.Zip(other.AddedObjects, (o1, o2) => o1.Equals(o2)).All(b => b)
                    && Container.Equals(other.Container);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddCommand<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AddedObjects.GetHashCode() * 397) ^ Container.GetHashCode();
            }
        }
    }
    
    public class AddCommand<TCloudItem, TCloudItemDiff> : ICommand 
            where TCloudItem : struct, ICloudItem 
            where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        protected readonly TCloudItemDiff[] AddedObjects;
        protected readonly IContainer<TCloudItem> Container;
        
        public AddCommand(IContainer<TCloudItem> container, TCloudItemDiff[] objects)
        {
            Container = container;
            AddedObjects = objects;
        }

        public virtual void Execute() => Container.AddRange(AddedObjects);
        public virtual void UnExecute() => Container.Remove(AddedObjects);

        protected bool Equals(AddCommand<TCloudItem, TCloudItemDiff> other)
        {
            return AddedObjects.Zip(other.AddedObjects, (o1, o2) => o1.Equals(o2)).All(b => b)
                    && Container.Equals(other.Container);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddCommand<TCloudItem, TCloudItemDiff>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AddedObjects.GetHashCode() * 397) ^ Container.GetHashCode();
            }
        }
    }
}
