using Elektronik.Common.Containers;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Commands.Generic
{
    public class ConnectableRemoveCommand<T> : RemoveCommand<T> where T: struct, ICloudItem
    {
        private readonly IList<(int, int)> _connections;
        private readonly IConnectableObjectsContainer<T> _container;

        public ConnectableRemoveCommand(IConnectableObjectsContainer<T> container, IEnumerable<T> objects) : base(container, objects)
        {
            _container = container;
            _connections = objects.SelectMany(o => _container.GetAllConnections(o)).ToList();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            if (_connections != null)
            {
                _container.AddConnections(_connections);
            }
        }
    }
}
