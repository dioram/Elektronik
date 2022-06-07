﻿using System.Collections.Generic;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;

namespace Elektronik.Plugins.Common.Commands.Generic
{
    public class RemoveConnectionsCommand<T> : ICommand where T: struct, ICloudItem
    {
        private readonly IList<(int, int)> _connections;
        private readonly IConnectableObjectsCloudContainer<T> _сontainer;

        public RemoveConnectionsCommand(IConnectableObjectsCloudContainer<T> сontainer, IList<(int, int)> connections)
        {
            _connections = connections;
            _сontainer = сontainer;
        }

        public void Execute() => _сontainer.RemoveConnections(_connections);
        public void UnExecute() => _сontainer.AddConnections(_connections);
    }
}