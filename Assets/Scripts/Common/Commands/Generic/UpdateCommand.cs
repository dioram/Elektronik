using Elektronik.Common.Containers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Elektronik.Common.Commands.Generic
{
    public class UpdateCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> m_objs2Restore;
        protected readonly ReadOnlyCollection<T> m_objs2Update;

        protected readonly IContainer<T> m_container;

        public UpdateCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            m_container = container;
            m_objs2Restore = new ReadOnlyCollection<T>(objects.Select(p => container[p]).ToList());
            m_objs2Update = new ReadOnlyCollection<T>(objects.ToList());
        }

        public virtual void Execute() => m_container.UpdateItems(m_objs2Update);
        public virtual void UnExecute() => m_container.UpdateItems(m_objs2Restore);
    }
}
