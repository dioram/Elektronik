using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;

namespace Elektronik.Offline.Commanders.Slam
{
    public class ObjectsCommander :  PackageViewUpdateCommander
    {
        public SlamMap map;

        public IPackageViewUpdateCommand GetCommand<T>(IContainer<T> map_, IEnumerable<T> objects, PacketPb.Types.ActionType action)
        {
            switch (action)
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

        public override void GetCommands(PacketPb packet, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            var command = GetCommand(packet);
            if (command != null)
            {
                command.Execute();
                commands.AddLast(command);
            }
            base.GetCommands(packet, commands);
        }

        protected virtual IPackageViewUpdateCommand GetCommand(PacketPb packet)
        {
            switch (packet.DataCase)
            {
                case PacketPb.DataOneofCase.PointsPacket:
                    return GetCommand(map.PointsContainer, packet.PointsPacket.Points.Select(p => (SlamPoint)p), packet.Action);
                case PacketPb.DataOneofCase.ObservationsPacket:
                    return GetCommand(map.ObservationsContainer, packet.ObservationsPacket.Observations.Select(o => (SlamObservation)o), packet.Action);
                case PacketPb.DataOneofCase.TrackedObjsPacket:
                    return GetCommand(map.TrackedObjsContainer, packet.TrackedObjsPacket.TrackedObjs, packet.Action);
                case PacketPb.DataOneofCase.ConnectionsPacket:
                    if (packet.ConnectionsPacket.Map == PacketPb.Types.ConnectionsPacket.Types.MapType.Points)
                        return GetCommand(map.PointsConnections, packet.ConnectionsPacket.Connections.Select(l => (SlamLine)l), packet.Action);
                    if (packet.ConnectionsPacket.Map == PacketPb.Types.ConnectionsPacket.Types.MapType.Observations)
                        return GetCommand(map.ObservationsConnections, packet.ConnectionsPacket.Connections.Select(l => (SlamLine)l), packet.Action);
                    return null;
                default:
                    return null;
            }
        }
    }
}
