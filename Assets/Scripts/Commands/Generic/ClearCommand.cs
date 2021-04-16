using System.Collections.ObjectModel;
using System.Linq;
using Elektronik.Containers;

namespace Elektronik.Commands.Generic
{
    public class ClearCommand<T> : ICommand
    {
        protected readonly ReadOnlyCollection<T> UndoObjects;
        private readonly IContainer<T> _container;

        public ClearCommand(IContainer<T> container)
        {
            _container = container;
            UndoObjects = new ReadOnlyCollection<T>(_container.ToArray());
        }

        public virtual void Execute() => _container.Clear();

        public virtual void UnExecute() => _container.AddRange(UndoObjects);
    }
}
