using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class ClearCommand<T> : IPackageViewUpdateCommand
    {
        private readonly IContainer<T> m_container;

        private readonly T[] m_undoObjects;
        public ClearCommand(IContainer<T> container)
        {
            m_container = container;
            m_undoObjects = m_container.GetAll();
        }

        public void Execute() => m_container.Clear();

        public void UnExecute() => m_container.Add(m_undoObjects);
    }
}
