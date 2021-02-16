using Elektronik.Common.Commands;
using Elektronik.Common.Commands.Generic;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;

namespace Elektronik.Offline.Commanders
{
    public class ObjectsCommander : Commander
    {
        public SlamMap map;
        public SlamInfinitePlanesContainer InfinitePlanesContainer;

        private ICommand GetCommandForConnectableObjects<T>(IConnectableObjectsContainer<T> map_,
            IEnumerable<T> objects, PacketPb packet) where T: ICloudItem
        {
            switch (packet.Action)
            {
                case PacketPb.Types.ActionType.Add:
                    return new AddCommand<T>(map_, objects);
                case PacketPb.Types.ActionType.Update:
                    var commands = new List<ICommand>();
                    if (packet.Connections != null)
                    {
                        var connections = packet.Connections.Data.Select(c => (c.Id1, c.Id2)).ToArray();
                        if (packet.Connections.Action == PacketPb.Types.Connections.Types.Action.Add)
                            commands.Add(new AddConnectionsCommand<T>(map_, connections));
                        if (packet.Connections.Action == PacketPb.Types.Connections.Types.Action.Remove)
                            commands.Add(new RemoveConnectionsCommand<T>(map_, connections));
                    }

                    commands.Add(new UpdateCommand<T>(map_, objects));
                    return new MacroCommand(commands);
                case PacketPb.Types.ActionType.Remove:
                    return new ConnectableRemoveCommand<T>(map_, objects);
                case PacketPb.Types.ActionType.Clear:
                    return new ConnectableClearCommand<T>(map_);
                default:
                    return null;
            }
        }
        
        private ICommand GetCommand<T>(IContainer<T> map_, IEnumerable<T> objects, PacketPb packet)
        {
            switch (packet.Action)
            {
                case PacketPb.Types.ActionType.Add:
                    return new AddCommand<T>(map_, objects);
                case PacketPb.Types.ActionType.Update:
                    return new UpdateCommand<T>(map_, objects);
                case PacketPb.Types.ActionType.Remove:
                    return new RemoveCommand<T>(map_, objects);
                case PacketPb.Types.ActionType.Clear:
                    return new ClearCommand<T>(map_);
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
                    return GetCommandForConnectableObjects(map.Points, packet.ExtractPoints(m_converter).ToList(),
                        packet);
                case PacketPb.DataOneofCase.Observations:
                    return GetCommandForConnectableObjects(map.Observations,
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