using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System.Collections.ObjectModel;

namespace Elektronik.Common.Commands.Generic
{
    public class ClearCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> m_undoObjects;
        private readonly IContainer<T> m_container;

        public ClearCommand(IContainer<T> container)
        {
            m_container = container;
            m_undoObjects = new ReadOnlyCollection<T>(m_container.GetAll());
        }

        public virtual void Execute() => m_container.Clear();

        public virtual void UnExecute() => m_container.Add(m_undoObjects);
    }
}
