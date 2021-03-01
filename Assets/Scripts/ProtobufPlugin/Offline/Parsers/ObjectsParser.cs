using Elektronik.Common.Commands;
using Elektronik.Common.Commands.Generic;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.ProtobufPlugin.Offline.Parsers
{
    public class ObjectsParser : PackageParser
    {
        private readonly IContainer<SlamInfinitePlane> _infinitePlanes;
        private readonly IConnectableObjectsContainer<SlamPoint> _points;
        private readonly IConnectableObjectsContainer<SlamObservation> _observations;

        public ObjectsParser(IContainer<SlamInfinitePlane> infinitePlanes,
                             IConnectableObjectsContainer<SlamPoint> points,
                             IConnectableObjectsContainer<SlamObservation> observations)
        {
            _infinitePlanes = infinitePlanes;
            _points = points;
            _observations = observations;
        }

        private ICommand GetCommandForConnectableObjects<T>(IConnectableObjectsContainer<T> map,
                                                            IEnumerable<T> objects, PacketPb packet)
                where T : struct, ICloudItem
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
                    switch (packet.Connections.Action)
                    {
                    case PacketPb.Types.Connections.Types.Action.Add:
                        commands.Add(new AddConnectionsCommand<T>(map, connections));
                        break;
                    case PacketPb.Types.Connections.Types.Action.Remove:
                        commands.Add(new RemoveConnectionsCommand<T>(map, connections));
                        break;
                    }
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
                where T : struct, ICloudItem
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

        public override ICommand GetCommand(PacketPb packet)
        {
            var command = ParsePacket(packet);
            if (command == null) return base.GetCommand(packet);
            return command;
        }

        private ICommand ParsePacket(PacketPb packet)
        {
            switch (packet.DataCase)
            {
            case PacketPb.DataOneofCase.Points:
                return GetCommandForConnectableObjects(_points,
                                                       packet.ExtractPoints(Converter).ToList(),
                                                       packet);
            case PacketPb.DataOneofCase.Observations:
                return GetCommandForConnectableObjects(_observations,
                                                       packet.ExtractObservations(
                                                                   Converter,
                                                                   ProtobufFilePlayer.OfflineSettings.ImagePath)
                                                               .ToList(), packet);
            case PacketPb.DataOneofCase.InfinitePlanes:
                return GetCommand(_infinitePlanes,
                                  packet.ExtractInfinitePlanes(Converter).ToList(), packet);
            default:
                return base.GetCommand(packet);
            }
        }
    }
}