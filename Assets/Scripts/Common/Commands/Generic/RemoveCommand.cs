using Elektronik.Common.Containers;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Elektronik.Common.Commands.Generic
{
    public class RemoveCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> m_objs2Remove;
        private readonly IContainer<T> m_container;

        public RemoveCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            m_container = container;
            m_objs2Remove = new ReadOnlyCollection<T>(objects.Select(p => m_container[p]).ToList());
        }

        public virtual void Execute() => m_container.Remove(m_objs2Remove);
        public virtual void UnExecute() => m_container.AddRange(m_objs2Remove);
    }
}
