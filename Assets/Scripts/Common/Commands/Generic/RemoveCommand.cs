using Elektronik.Common.Containers;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Commands.Generic
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
}
