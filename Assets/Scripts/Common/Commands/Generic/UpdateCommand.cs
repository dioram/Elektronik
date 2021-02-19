using Elektronik.Common.Containers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Elektronik.Common.Commands.Generic
{
    public class UpdateCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> Objs2Restore;
        protected readonly ReadOnlyCollection<T> Objs2Update;

        protected readonly IContainer<T> Container;

        public UpdateCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            Container = container;
            Objs2Restore = new ReadOnlyCollection<T>(objects.Select(p => container[p]).ToList());
            Objs2Update = new ReadOnlyCollection<T>(objects.ToList());
        }

        public virtual void Execute() => Container.UpdateItems(Objs2Update);
        public virtual void UnExecute() => Container.UpdateItems(Objs2Restore);
    }
}
