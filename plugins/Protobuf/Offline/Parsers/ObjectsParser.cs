using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.Commands;
using Elektronik.Plugins.Common.Commands.Generic;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Plugins.Common.Parsing;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline.Parsers
{
    public class ObjectsParser : DataParser<PacketPb>
    {
        private readonly ICloudContainer<SlamPlane> _planes;
        private readonly IConnectableObjectsCloudContainer<SlamPoint> _points;
        private readonly IConnectableObjectsCloudContainer<SlamObservation> _observations;
        private readonly string _imagePath;

        public ObjectsParser(ICloudContainer<SlamPlane> planes,
                             IConnectableObjectsCloudContainer<SlamPoint> points,
                             IConnectableObjectsCloudContainer<SlamObservation> observations,
                             string imagePath)
        {
            _planes = planes;
            _points = points;
            _observations = observations;
            _imagePath = imagePath;
        }

        public override ICommand? GetCommand(PacketPb packet) => ParsePacket(packet) ?? base.GetCommand(packet);

        #region Private

        private ICommand? ParsePacket(PacketPb packet)
        {
            switch (packet.DataCase)
            {
            case PacketPb.DataOneofCase.Points:
                return GetCommandForConnectableObjects(_points, packet.ExtractPoints(Converter), packet);
            case PacketPb.DataOneofCase.Observations:
                return GetCommandForConnectableObjects(_observations, packet.ExtractObservations(Converter, _imagePath),
                                                       packet);
            case PacketPb.DataOneofCase.Planes:
                return GetCommand(_planes, packet.ExtractPlanes(Converter), packet);
            default:
                return base.GetCommand(packet);
            }
        }

        private ICommand? GetCommandForConnectableObjects<TCloudItem, TCloudItemDiff>(
            IConnectableObjectsCloudContainer<TCloudItem> map,
            TCloudItemDiff[] objects, PacketPb packet)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            if (objects.Length == 0) return null;
            switch (packet.Action)
            {
            case PacketPb.Types.ActionType.Add:
                return new AddCommand<TCloudItem, TCloudItemDiff>(map, objects);
            case PacketPb.Types.ActionType.Update:
                return GetUpdateCommand(map, objects, packet);
            case PacketPb.Types.ActionType.Remove:
                return new ConnectableRemoveCommand<TCloudItem, TCloudItemDiff>(map, objects);
            case PacketPb.Types.ActionType.Clear:
                return new ConnectableClearCommand<TCloudItem>(map);
            default:
                return null;
            }
        }

        private ICommand? GetCommand<TCloudItem, TCloudItemDiff>(ICloudContainer<TCloudItem> map,
                                                                 TCloudItemDiff[] objects,
                                                                 PacketPb packet)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            if (objects.Length == 0) return null;
            return packet.Action switch
            {
                PacketPb.Types.ActionType.Add => new AddCommand<TCloudItem, TCloudItemDiff>(map, objects),
                PacketPb.Types.ActionType.Update => new UpdateCommand<TCloudItem, TCloudItemDiff>(map, objects),
                PacketPb.Types.ActionType.Remove => new RemoveCommand<TCloudItem, TCloudItemDiff>(map, objects),
                PacketPb.Types.ActionType.Clear => new ClearCommand<TCloudItem>(map),
                _ => null
            };
        }

        private ICommand GetUpdateCommand<TCloudItem, TCloudItemDiff>(IConnectableObjectsCloudContainer<TCloudItem> map,
                                                                      TCloudItemDiff[] objects, PacketPb packet)
                where TCloudItem : struct, ICloudItem
                where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
        {
            var commands = new List<ICommand>();
            if (packet.Connections != null)
            {
                var connections = packet.Connections.Data.Select(c => (c.Id1, c.Id2)).ToList();
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
        }

        #endregion
    }
}