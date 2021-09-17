using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Recorders
{
    public static class EventArgsToProtobufExt
    {
        public static PacketPb ToProtobuf(this AddedEventArgs<SlamPoint> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points()
            };
            packet.Points.Data.AddRange(data.AddedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this AddedEventArgs<SlamLine> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Lines = new PacketPb.Types.Lines()
            };
            packet.Lines.Data.AddRange(data.AddedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this AddedEventArgs<SlamObservation> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations()
            };
            packet.Observations.Data.AddRange(data.AddedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this AddedEventArgs<SlamTrackedObject> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                TrackedObjs = new PacketPb.Types.TrackedObjs()
            };
            packet.TrackedObjs.Data.AddRange(data.AddedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this AddedEventArgs<SlamPlane> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.AddRange(data.AddedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this UpdatedEventArgs<SlamPoint> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points()
            };
            packet.Points.Data.AddRange(data.UpdatedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this UpdatedEventArgs<SlamLine> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Lines = new PacketPb.Types.Lines()
            };
            packet.Lines.Data.AddRange(data.UpdatedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this UpdatedEventArgs<SlamObservation> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations()
            };
            packet.Observations.Data.AddRange(data.UpdatedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this UpdatedEventArgs<SlamTrackedObject> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs()
            };
            packet.TrackedObjs.Data.AddRange(data.UpdatedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this UpdatedEventArgs<SlamPlane> data, ICSConverter converter)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.AddRange(data.UpdatedItems.Select(s => s.ToProtobuf(converter)));
            return packet;
        }

        public static PacketPb ToProtobuf(this RemovedEventArgs<SlamPoint> data)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.AddRange(data.RemovedItems.Select(s => s.ToProtobuf()));
            return packet;
        }

        public static PacketPb ToProtobuf(this RemovedEventArgs<SlamLine> data)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Lines = new PacketPb.Types.Lines(),
            };
            packet.Lines.Data.AddRange(data.RemovedItems.Select(s => s.ToProtobuf()));
            return packet;
        }

        public static PacketPb ToProtobuf(this RemovedEventArgs<SlamObservation> data)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Observations = new PacketPb.Types.Observations(),
            };
            packet.Observations.Data.AddRange(data.RemovedItems.Select(s => s.ToProtobuf()));
            return packet;
        }

        public static PacketPb ToProtobuf(this RemovedEventArgs<SlamTrackedObject> data)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            packet.TrackedObjs.Data.AddRange(data.RemovedItems.Select(s => s.ToProtobuf()));
            return packet;
        }

        public static PacketPb ToProtobuf(this RemovedEventArgs<SlamPlane> data)
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.AddRange(data.RemovedItems.Select(s => s.ToProtobuf()));
            return packet;
        }
    }
}