using Elektronik.Common.Containers;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Elektronik.Common.Commands.Generic
{
    public class AddCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> AddedObjects;
        protected readonly IContainer<T> Container;
        
        public AddCommand(
            IContainer<T> container,
            IEnumerable<T> objects)
        {
            Container = container;
            AddedObjects = new ReadOnlyCollection<T>(objects.ToList());
        }        

        public virtual void Execute() => Container.AddRange(AddedObjects);
        public virtual void UnExecute() => Container.Remove(AddedObjects);
    }
}
