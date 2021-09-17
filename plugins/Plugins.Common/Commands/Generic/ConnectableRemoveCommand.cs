using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.DataDiff;

namespace Elektronik.Plugins.Common.Commands.Generic
{
    public class ConnectableRemoveCommand<T> : RemoveCommand<T> where T : struct, ICloudItem
    {
        private readonly IList<(int, int)> _connections;
        private readonly IConnectableObjectsContainer<T> _container;

        public ConnectableRemoveCommand(IConnectableObjectsContainer<T> container, IList<T> objects)
                : base(container, objects)
        {
            _container = container;
            _connections = objects.SelectMany(o => _container.GetAllConnections(o)).ToList();
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
        private readonly IConnectableObjectsContainer<TCloudItem> _container;

        public ConnectableRemoveCommand(IConnectableObjectsContainer<TCloudItem> container,
                                        IList<TCloudItemDiff> objects)
                : base(container, objects)
        {
            _container = container;
            _connections = objects.SelectMany(o => _container.GetAllConnections(o)).ToList();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            _container.AddConnections(_connections);
        }
    }
}