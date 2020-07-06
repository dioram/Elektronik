using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class ClearCommand<T> : IPackageViewUpdateCommand
    {
        protected readonly T[] m_undoObjects;
        protected readonly IContainer<T> m_container;

        public ClearCommand(IContainer<T> container)
        {
            m_container = container;
            m_undoObjects = m_container.GetAll();
        }

        public virtual void Execute() => m_container.Clear();

        public virtual void UnExecute() => m_container.Add(m_undoObjects);
    }
}
