using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using System.Linq;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class AddCommand<T> : IPackageViewUpdateCommand
    {
        private readonly T[] m_addedObjects;
        private readonly IContainer<T> m_container;

        public AddCommand(
            IContainer<T> container,
            ActionDataPackage<T> slamEvent)
        {
            m_container = container;
            m_addedObjects = slamEvent.Objects;
        }

        public void Execute()
        {
            if (m_addedObjects != null)
            {
                foreach (var obj in m_addedObjects)
                {
                    m_container.Add(obj);
                }
            }
        }

        public void UnExecute()
        {
            if (m_addedObjects != null)
            {
                foreach (var obj in m_addedObjects)
                {
                    m_container.Remove(obj);
                }
            }
        }
    }
}
