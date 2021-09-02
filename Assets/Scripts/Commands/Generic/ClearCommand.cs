using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class ClearCommand<T> : ICommand where T : ICloudItem
    {
        protected readonly T[] UndoObjects;
        private readonly IContainer<T> _container;

        public ClearCommand(IContainer<T> container)
        {
            _container = container;
            UndoObjects = _container.ToArray();
        }

        public virtual void Execute() => _container.Clear();

        public virtual void UnExecute() => _container.AddRange(UndoObjects);
    }
}
