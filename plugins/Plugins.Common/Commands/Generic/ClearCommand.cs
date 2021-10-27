using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;

namespace Elektronik.Plugins.Common.Commands.Generic
{
    public class ClearCommand<T> : ICommand where T : struct, ICloudItem
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
