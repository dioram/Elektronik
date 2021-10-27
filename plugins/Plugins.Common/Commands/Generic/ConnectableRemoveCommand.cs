using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.DataDiff;

namespace Elektronik.Plugins.Common.Commands.Generic
{
    public class ConnectableRemoveCommand<T> : RemoveCommand<T> where T : struct, ICloudItem
    {
        private readonly IList<(int, int)> _connections;
        private readonly IConnectableObjectsCloudContainer<T> _container;

        public ConnectableRemoveCommand(IConnectableObjectsCloudContainer<T> container, T[] objects)
                : base(container, objects)
        {
            _container = container;
            _connections = objects.SelectMany(o => _container.GetConnections(o).Select(c => (o.Id, c))).ToList();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            _container.AddConnections(_connections);
        }
    }

    public class ConnectableRemoveCommand<TCloudItem, TCloudItemDiff> : RemoveCommand<TCloudItem, TCloudItemDiff>
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        private readonly IList<(int, int)> _connections;
        private readonly IConnectableObjectsCloudContainer<TCloudItem> _container;

        public ConnectableRemoveCommand(IConnectableObjectsCloudContainer<TCloudItem> container,
                                        TCloudItemDiff[] objects)
                : base(container, objects)
        {
            _container = container;
            _connections = objects.SelectMany(o => _container.GetConnections(o.Id).Select(c => (o.Id, c))).ToList();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            _container.AddConnections(_connections);
        }
    }
}