using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Elektronik.Common.Commands.Generic
{
    public class AddCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> m_addedObjects;
        protected readonly IContainer<T> m_container;
        
        public AddCommand(
            IContainer<T> container,
            IEnumerable<T> objects)
        {
            m_container = container;
            m_addedObjects = new ReadOnlyCollection<T>(objects.ToList());
        }        

        public virtual void Execute() => m_container.Add(m_addedObjects);
        public virtual void UnExecute() => m_container.Remove(m_addedObjects);
    }
}
