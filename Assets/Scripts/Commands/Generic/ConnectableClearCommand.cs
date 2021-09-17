using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;

namespace Elektronik.Commands.Generic
{
    public class ConnectableClearCommand<T> : ClearCommand<T> where T: struct, ICloudItem
    {
        private readonly (int, int)[] _connections;
        private readonly IConnectableObjectsContainer<T> _container;

        public ConnectableClearCommand(IConnectableObjectsContainer<T> container) : base(container)
        {
            _container = container;
            _connections = container.Connections.Select(l => (l.Point1.Id, l.Point2.Id)).ToArray();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            _container.AddConnections(_connections);
        }
    }
}
