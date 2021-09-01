using System.Collections.Generic;
using System.Linq;
using Elektronik.Commands;
using Elektronik.Commands.Generic;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline.Parsers
{
    public class ObjectsParser : DataParser<PacketPb>
    {
        private readonly IContainer<SlamInfinitePlane> _infinitePlanes;
        private readonly IConnectableObjectsContainer<SlamPoint> _points;
        private readonly IConnectableObjectsContainer<SlamObservation> _observations;
        private readonly string _imagePath;

        public ObjectsParser(IContainer<SlamInfinitePlane> infinitePlanes,
                             IConnectableObjectsContainer<SlamPoint> points,
                             IConnectableObjectsContainer<SlamObservation> observations,
                             string imagePath)
        {
            _infinitePlanes = infinitePlanes;
            _points = points;
            _observations = observations;
            _imagePath = imagePath;
        }

        private ICommand? GetCommandForConnectableObjects<TCloudItem, TCloudItemDiff>(
            IConnectableObjectsContainer<TCloudItem> map,
            IList<TCloudItemDiff>? objects, PacketPb packet)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            if (objects is null || objects.Count == 0) return null;
            switch (packet.Action)
            {
            case PacketPb.Types.ActionType.Add:
                return new AddCommand<TCloudItem, TCloudItemDiff>(map, objects);
            case PacketPb.Types.ActionType.Update:
                var commands = new List<ICommand>();
                if (packet.Connections != null)
                {
                    var connections = packet.Connections.Data.Select(c => (c.Id1, c.Id2)).ToArray();
                    switch (packet.Connections.Action)
                    {
                    case PacketPb.Types.Connections.Types.Action.Add:
                        commands.Add(new AddConnectionsCommand<TCloudItem>(map, connections));
                        break;
                    case PacketPb.Types.Connections.Types.Action.Remove:
                        commands.Add(new RemoveConnectionsCommand<TCloudItem>(map, connections));
                        break;
                    }
                }

                commands.Add(new UpdateCommand<TCloudItem, TCloudItemDiff>(map, objects));
                return new MacroCommand(commands);
            case PacketPb.Types.ActionType.Remove:
                return new ConnectableRemoveCommand<TCloudItem, TCloudItemDiff>(map, objects);
            case PacketPb.Types.ActionType.Clear:
                return new ConnectableClearCommand<TCloudItem>(map);
            default:
                return null;
            }
        }

        private ICommand? GetCommand<TCloudItem, TCloudItemDiff>(IContainer<TCloudItem> map, 
                                                                 IList<TCloudItemDiff>? objects,
                                                                 PacketPb packet)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            if (objects is null || objects.Count == 0) return null;
            switch (packet.Action)
            {
            case PacketPb.Types.ActionType.Add:
                return new AddCommand<TCloudItem, TCloudItemDiff>(map, objects);
            case PacketPb.Types.ActionType.Update:
                return new UpdateCommand<TCloudItem, TCloudItemDiff>(map, objects);
            case PacketPb.Types.ActionType.Remove:
                return new RemoveCommand<TCloudItem, TCloudItemDiff>(map, objects);
            case PacketPb.Types.ActionType.Clear:
                return new ClearCommand<TCloudItem>(map);
            default:
                return null;
            }
        }

        public override ICommand? GetCommand(PacketPb packet)
        {
            var command = ParsePacket(packet);
            if (command == null) return base.GetCommand(packet);
            return command;
        }

        private ICommand? ParsePacket(PacketPb packet)
        {
            switch (packet.DataCase)
            {
            case PacketPb.DataOneofCase.Points:
                return GetCommandForConnectableObjects(_points,
                                                       packet.ExtractPoints(Converter).ToList(),
                                                       packet);
            case PacketPb.DataOneofCase.Observations:
                return GetCommandForConnectableObjects(_observations,
                                                       packet.ExtractObservations(Converter, _imagePath).ToList(),
                                                       packet);
            case PacketPb.DataOneofCase.InfinitePlanes:
                return GetCommand(_infinitePlanes,
                                  packet.ExtractInfinitePlanes(Converter).ToList(), packet);
            default:
                return base.GetCommand(packet);
            }
        }
    }
}