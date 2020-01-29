using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class RemoveCommand<T> : IPackageViewUpdateCommand
    {
        private readonly T[] m_objs2Remove;

        private readonly IContainer<T> m_container;

        public RemoveCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            m_container = container;
            if (objects != null)
            {
                m_objs2Remove = objects
                    .Select(p => m_container[p])
                    .ToArray();
            }
        }

        public void Execute()
        {
            if (m_objs2Remove != null)
            {
                m_container.Remove(m_objs2Remove);
            }
        }

        public void UnExecute()
        {
            if (m_objs2Remove != null)
            {
                m_container.Add(m_objs2Remove);
            }
        }
    }
}
