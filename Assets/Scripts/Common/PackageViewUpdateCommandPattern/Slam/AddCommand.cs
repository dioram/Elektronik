using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using System.Linq;
using System.Collections.Generic;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class AddCommand<T> : IPackageViewUpdateCommand
    {
        protected readonly T[] m_addedObjects;
        protected readonly IContainer<T> m_container;
        
        public AddCommand(
            IContainer<T> container,
            IEnumerable<T> objects)
        {
            m_container = container;
            m_addedObjects = objects.ToArray();
        }        

        public virtual void Execute()
        {
            if (m_addedObjects != null)
            {
                m_container.Add(m_addedObjects);
            }
        }

        public virtual void UnExecute()
        {
            if (m_addedObjects != null)
            {
                m_container.Remove(m_addedObjects);
            }
        }
    }
}
