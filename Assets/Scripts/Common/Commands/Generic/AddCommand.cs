using Elektronik.Common.Containers;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Elektronik.Common.Commands.Generic
{
    public class AddCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> m_addedObjects;
        protected readonly IContainer<T> m_container;
        
        public AddCommand(
            IContainer<T> container,
            IEnumerable<T> objects)
        {
            m_container = container;
            m_addedObjects = new ReadOnlyCollection<T>(objects.ToList());
        }        

        public virtual void Execute() => m_container.AddRange(m_addedObjects);
        public virtual void UnExecute() => m_container.Remove(m_addedObjects);
    }
}
