using Elektronik.Common.Commands;
using Elektronik.Common.Commands.Generic;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Offline.Commanders
{
    public class ObjectsCommander : Commander
    {
        public SlamInfinitePlanesContainer InfinitePlanesContainer;
        public ConnectableObjectsContainer<SlamPoint> ConnectablePointsContainer;
        public ConnectableObjectsContainer<SlamObservation> ConnectableObservationsContainer;

        private ICommand GetCommandForConnectableObjects<T>(IConnectableObjectsContainer<T> map,
                                                            IEnumerable<T> objects, PacketPb packet)
                where T : ICloudItem
        {
            switch (packet.Action)
            {
            case PacketPb.Types.ActionType.Add:
                return new AddCommand<T>(map, objects);
            case PacketPb.Types.ActionType.Update:
                var commands = new List<ICommand>();
                if (packet.Connections != null)
                {
                    var connections = packet.Connections.Data.Select(c => (c.Id1, c.Id2)).ToArray();
                    if (packet.Connections.Action == PacketPb.Types.Connections.Types.Action.Add)
                        commands.Add(new AddConnectionsCommand<T>(map, connections));
                    if (packet.Connections.Action == PacketPb.Types.Connections.Types.Action.Remove)
                        commands.Add(new RemoveConnectionsCommand<T>(map, connections));
                }

                commands.Add(new UpdateCommand<T>(map, objects));
                return new MacroCommand(commands);
            case PacketPb.Types.ActionType.Remove:
                return new ConnectableRemoveCommand<T>(map, objects);
            case PacketPb.Types.ActionType.Clear:
                return new ConnectableClearCommand<T>(map);
            default:
                return null;
            }
        }

        private ICommand GetCommand<T>(IContainer<T> map, IEnumerable<T> objects, PacketPb packet)
        {
            switch (packet.Action)
            {
            case PacketPb.Types.ActionType.Add:
                return new AddCommand<T>(map, objects);
            case PacketPb.Types.ActionType.Update:
                return new UpdateCommand<T>(map, objects);
            case PacketPb.Types.ActionType.Remove:
                return new RemoveCommand<T>(map, objects);
            case PacketPb.Types.ActionType.Clear:
                return new ClearCommand<T>(map);
            default:
                return null;
            }
        }

        public override void GetCommands(PacketPb packet, in LinkedList<ICommand> commands)
        {
            var command = GetCommand(packet);
            if (command != null)
            {
                command.Execute();
                commands.AddLast(command);
            }

            base.GetCommands(packet, commands);
        }

        protected virtual ICommand GetCommand(PacketPb packet)
        {
            switch (packet.DataCase)
            {
            case PacketPb.DataOneofCase.Points:
                return GetCommandForConnectableObjects(ConnectablePointsContainer,
                                                       packet.ExtractPoints(m_converter).ToList(),
                                                       packet);
            case PacketPb.DataOneofCase.Observations:
                return GetCommandForConnectableObjects(ConnectableObservationsContainer,
                                                       packet.ExtractObservations(m_converter).ToList(), packet);
            case PacketPb.DataOneofCase.InfinitePlanes:
                return GetCommand(InfinitePlanesContainer,
                                  packet.ExtractInfinitePlanes(m_converter).ToList(), packet);
            default:
                return null;
            }
        }
    }
}