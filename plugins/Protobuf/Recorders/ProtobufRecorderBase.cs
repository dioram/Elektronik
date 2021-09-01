using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRecorderBase
    {
        #region Protected

        protected PacketPb CreateAddedPacket<TCloudItem>(IList<TCloudItem> args, int timestamp, ICSConverter converter)
                where TCloudItem : struct, ICloudItem
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Timestamp = timestamp,
            };
            SetData(args, packet, converter);
            return packet;
        }

        protected PacketPb CreateUpdatedPacket<TCloudItem>(IList<TCloudItem> args, int timestamp,
                                                           ICSConverter converter)
                where TCloudItem : struct, ICloudItem
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Timestamp = timestamp,
            };
            SetData(args, packet, converter);
            return packet;
        }

        protected PacketPb CreateRemovedPacket<TCloudItem>(IList<int> args, int timestamp)
                where TCloudItem : struct, ICloudItem
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Timestamp = timestamp,
            };
            SetData<TCloudItem>(args, packet);
            return packet;
        }

        protected PacketPb CreateConnectionsUpdatedPacket<TCloudItem>(IEnumerable<(int id1, int id2)> items,
                                                                      int timestamp)
                where TCloudItem : struct, ICloudItem
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Timestamp = timestamp,
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            SetData<TCloudItem>(new List<int>(), packet);
            packet.Connections.Data.Add(items.Select(i => new ConnectionPb {Id1 = i.id1, Id2 = i.id2}));
            return packet;
        }

        protected PacketPb CreateConnectionsRemovedPacket<TCloudItem>(IEnumerable<(int id1, int id2)> items,
                                                                      int timestamp)
                where TCloudItem : struct, ICloudItem
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Timestamp = timestamp,
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
            };
            SetData<TCloudItem>(new List<int>(), packet);

            packet.Connections.Data.Add(items.Select(i => new ConnectionPb {Id1 = i.id1, Id2 = i.id2}));
            return packet;
        }

        #endregion

        #region Private

        private void SetData<TCloudItem>(IList<TCloudItem> items, PacketPb packet, ICSConverter converter)
                where TCloudItem : struct, ICloudItem
        {
            var first = items.FirstOrDefault();
            switch (first)
            {
            case SlamPoint _:
                packet.Points = new PacketPb.Types.Points();
                packet.Points.Data.AddRange(items.OfType<SlamPoint>().Select(s => s.ToProtobuf(converter)));
                break;
            case SlamObservation _:
                packet.Observations = new PacketPb.Types.Observations();
                packet.Observations.Data.AddRange(items.OfType<SlamObservation>().Select(s => s.ToProtobuf(converter)));
                break;
            case SlamTrackedObject _:
                packet.TrackedObjs = new PacketPb.Types.TrackedObjs();
                packet.TrackedObjs.Data.AddRange(items.OfType<SlamTrackedObject>()
                                                         .Select(s => s.ToProtobuf(converter)));
                break;
            case SlamInfinitePlane _:
                packet.InfinitePlanes = new PacketPb.Types.InfinitePlanes();
                packet.InfinitePlanes.Data.AddRange(items.OfType<SlamInfinitePlane>()
                                                            .Select(s => s.ToProtobuf(converter)));
                break;
            case SlamLine _:
                packet.Lines = new PacketPb.Types.Lines();
                packet.Lines.Data.AddRange(items.OfType<SlamLine>().Select(s => s.ToProtobuf(converter)));
                break;
            }
        }

        private void SetData<TCloudItem>(IList<int> args, PacketPb packet) where TCloudItem : struct, ICloudItem
        {
            if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamPoint)))
            {
                packet.Points = new PacketPb.Types.Points();
                packet.Points.Data.AddRange(args.Select(i => new PointPb {Id = i}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamObservation)))
            {
                packet.Observations = new PacketPb.Types.Observations();
                packet.Observations.Data.AddRange(args.Select(i => new ObservationPb {Point = new PointPb {Id = i}}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamTrackedObject)))
            {
                packet.TrackedObjs = new PacketPb.Types.TrackedObjs();
                packet.TrackedObjs.Data.AddRange(args.Select(i => new TrackedObjPb {Id = i}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamInfinitePlane)))
            {
                packet.InfinitePlanes = new PacketPb.Types.InfinitePlanes();
                packet.InfinitePlanes.Data.AddRange(args.Select(i => new InfinitePlanePb {Id = i}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamLine)))
            {
                packet.Lines = new PacketPb.Types.Lines();
                packet.Lines.Data.AddRange(args.Select(_ => new LinePb
                {
                    Pt1 = new PointPb {Id = 1},
                    Pt2 = new PointPb {Id = 2},
                }));
            }
        }

        #endregion
    }
}