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
        private readonly T[] m_objs2Restore;
        private readonly T[] m_objs2Update;

        private readonly IContainer<T> m_container;

        public UpdateCommand(IContainer<T> container, ActionDataPackage<T> slamEvent)
        {
            m_container = container;
            if (slamEvent.Objects != null)
            {
                m_objs2Restore = slamEvent.Objects.Select(p => container[p]).ToArray();
                m_objs2Update = slamEvent.Objects;
            }
        }

        public void Execute()
        {
            foreach (var obj in m_objs2Update)
            {
                m_container.Update(obj);
            }
        }

        public void UnExecute()
        {
            foreach (var obj in m_objs2Restore)
            {
                m_container.Update(obj);
            }
        }
    }
}
