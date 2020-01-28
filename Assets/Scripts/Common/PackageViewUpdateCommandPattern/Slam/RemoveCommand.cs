using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class RemoveCommand<T> : IPackageViewUpdateCommand
    {
        private readonly T[] m_objs2Remove;

        private readonly IContainer<T> m_container;

        public RemoveCommand(IContainer<T> container, ActionDataPackage<T> slamEvent)
        {
            m_container = container;
            if (slamEvent.Objects != null)
            {
                m_objs2Remove = slamEvent.Objects
                    .Select(p => m_container[p])
                    .ToArray();
            }
        }

        public void Execute()
        {
            if (m_objs2Remove != null)
            {
                foreach (var obj in m_objs2Remove)
                {
                    m_container.Remove(obj);
                }
            }
        }

        public void UnExecute()
        {
            if (m_objs2Remove != null)
            {
                foreach (var obj in m_objs2Remove)
                {
                    m_container.Add(obj);
                }
            }
        }
    }
}
