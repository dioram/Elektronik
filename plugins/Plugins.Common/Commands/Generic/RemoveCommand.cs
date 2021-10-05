using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.DataDiff;

namespace Elektronik.Plugins.Common.Commands.Generic
{
    public class RemoveCommand<T> : ICommand where T : struct, ICloudItem
    {
        protected readonly T[] Objs2Remove;
        private readonly IContainer<T> _container;

        public RemoveCommand(IContainer<T> container, T[] objects)
        {
            _container = container;
            Objs2Remove = objects;
        }

        public virtual void Execute() => _container.Remove(Objs2Remove);
        public virtual void UnExecute() => _container.AddRange(Objs2Remove);

        protected bool Equals(RemoveCommand<T> other)
        {
            return Objs2Remove.Zip(other.Objs2Remove, (o1, o2) => o1.Id.Equals(o2.Id)).All(b => b)
                    && _container.Equals(other._container);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RemoveCommand<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Objs2Remove.GetHashCode() * 397) ^ Objs2Remove.GetHashCode();
            }
        }
    }

    public class RemoveCommand<TCloudItem, TCloudItemDiff> : ICommand
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        protected readonly TCloudItemDiff[] Objs2Remove;
        protected TCloudItem[] Objs2Add = Array.Empty<TCloudItem>();
        private readonly IContainer<TCloudItem> _container;

        public RemoveCommand(IContainer<TCloudItem> container, TCloudItemDiff[] objects)
        {
            _container = container;
            Objs2Remove = objects;
        }

        public virtual void Execute()
        {
            Objs2Add = _container.Remove(Objs2Remove.Select(o => o.Id).ToArray()).ToArray();
        }

        public virtual void UnExecute() => _container.AddRange(Objs2Add);

        protected bool Equals(RemoveCommand<TCloudItem, TCloudItemDiff> other)
        {
            return Objs2Remove.Zip(other.Objs2Remove, (o1, o2) => o1.Id.Equals(o2.Id)).All(b => b)
                    && _container.Equals(other._container);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RemoveCommand<TCloudItem, TCloudItemDiff>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Objs2Remove.GetHashCode() * 397) ^ Objs2Remove.GetHashCode();
            }
        }
    }
}