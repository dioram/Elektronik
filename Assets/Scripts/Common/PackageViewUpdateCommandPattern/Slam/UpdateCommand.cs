using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class UpdateCommand<T> : IPackageViewUpdateCommand
    {
        protected readonly T[] m_objs2Restore;
        protected readonly T[] m_objs2Update;

        protected readonly IContainer<T> m_container;

        public UpdateCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            m_container = container;
            if (objects != null)
            {
                m_objs2Restore = objects.Select(p => container[p]).ToArray();
                m_objs2Update = objects.ToArray();
            }
        }

        public virtual void Execute()
        {
            if (m_objs2Update != null)
            {
                m_container.Update(m_objs2Update);
            }
        }

        public virtual void UnExecute()
        {
            if (m_objs2Restore != null)
            {
                m_container.Update(m_objs2Restore);
            }
                
        }
    }
}
