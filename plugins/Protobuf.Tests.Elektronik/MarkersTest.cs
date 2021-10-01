using System.Linq;
using Elektronik.Protobuf.Data;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class MarkersTest : TestsBase
    {
        private readonly MarkerPb[] _map;
        private static readonly string Filename = $"{nameof(MarkersTest)}.dat";

        public MarkersTest()
        {
            _map = Enumerable.Range(0, 5).Select(id => new MarkerPb()
            {
                Id = id,
                Message = $"{id}",
                Primitive = MarkerPb.Types.Type.Cube
            }).ToArray();

            _map[0].Position = new Vector3Pb { X = 7.5010533332824707, Y = 1.4059672355651855, Z = 8.4716329574584961 };
            _map[0].Orientation = new Vector4Pb { X = 0.70710676908493042, W = 0.70710676908493042 };
            _map[0].Message = "chair";
            _map[0].Color = new ColorPb { R = 255 };
            _map[1].Position = new Vector3Pb { X = 1 };
            _map[1].Color = new ColorPb { G = 255 };
            _map[2].Position = new Vector3Pb { X = -2.5, Y = -2.5 };
            _map[2].Color = new ColorPb { B = 255 };
            _map[3].Position = new Vector3Pb { X = -2.5, Y = 0 };
            _map[3].Color = new ColorPb { R = 255, G = 255 };
            _map[4].Position = new Vector3Pb { X = 2.5, Y = 0 };
            _map[4].Color = new ColorPb { R = 255, B = 255 };
        }

        [Test, Order(1), Explicit]
        public void Create()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Markers = new PacketPb.Types.Markers(),
            };
            packet.Markers.Data.Add(_map);

            SendAndCheck(packet, Filename);
        }

        [Test, Order(2), Explicit]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };

            packet.Markers.Data.Add(new MarkerPb { Id = 0, Position = new Vector3Pb { X = 1.5f } });
            packet.Markers.Data.Add(new MarkerPb { Id = 1, Position = new Vector3Pb { X = 3.5f } });
            packet.Markers.Data.Add(new MarkerPb { Id = 2, Position = new Vector3Pb { X = 5.5f } });
            packet.Markers.Data.Add(new MarkerPb { Id = 3, Position = new Vector3Pb { X = 7.5f } });
            packet.Markers.Data.Add(new MarkerPb { Id = 4, Position = new Vector3Pb { X = 9.5f } });

            SendAndCheck(packet, Filename);
        }

        [Test, Order(3), Explicit]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };

            packet.Markers.Data.Add(new MarkerPb { Id = 0, Color = new ColorPb { R = 255, G = 255, B = 255 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 1, Color = new ColorPb { R = 255, G = 255, B = 255 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 2, Color = new ColorPb { R = 255, G = 255, B = 255 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 3, Color = new ColorPb { R = 255, G = 255, B = 255 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 4, Color = new ColorPb { R = 255, G = 255, B = 255 } });

            SendAndCheck(packet, Filename);
        }

        [Test, Order(4), Explicit]
        public void UpdateScales()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };

            packet.Markers.Data.Add(new MarkerPb { Id = 0, Scale = new Vector3Pb { X = 0.1, Y = 2, Z = 1 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 1, Scale = new Vector3Pb { X = 0.1, Y = 2, Z = 1 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 2, Scale = new Vector3Pb { X = 0.1, Y = 2, Z = 1 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 3, Scale = new Vector3Pb { X = 0.1, Y = 2, Z = 1 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 4, Scale = new Vector3Pb { X = 0.1, Y = 2, Z = 1 } });

            SendAndCheck(packet, Filename);
        }

        [Test, Order(5), Explicit]
        public void UpdateOrientations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };

            packet.Markers.Data.Add(new MarkerPb { Id = 0, Orientation = new Vector4Pb { Y = 0.70710676908493042, W = 0.70710676908493042 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 1, Orientation = new Vector4Pb { Y = 0.70710676908493042, W = 0.70710676908493042 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 2, Orientation = new Vector4Pb { Y = 0.70710676908493042, W = 0.70710676908493042 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 3, Orientation = new Vector4Pb { Y = 0.70710676908493042, W = 0.70710676908493042 } });
            packet.Markers.Data.Add(new MarkerPb { Id = 4, Orientation = new Vector4Pb { Y = 0.70710676908493042, W = 0.70710676908493042 } });

            SendAndCheck(packet, Filename);
        }

        [Test, Order(6), Explicit]
        public void UpdateMessages()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };

            packet.Markers.Data.Add(new MarkerPb { Id = 0, Message = "Cube 0" });
            packet.Markers.Data.Add(new MarkerPb { Id = 1, Message = "Cube 1" });
            packet.Markers.Data.Add(new MarkerPb { Id = 2, Message = "Cube 2" });
            packet.Markers.Data.Add(new MarkerPb { Id = 3, Message = "Cube 3" });
            packet.Markers.Data.Add(new MarkerPb { Id = 4, Message = "Cube 4" });

            SendAndCheck(packet, Filename);
        }

        [Test, Order(7), Explicit]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Remove,
                Markers = new PacketPb.Types.Markers(),
            };

            packet.Markers.Data.Add(new[] { _map[1], _map[3] });

            SendAndCheck(packet, Filename);
        }

        [Test, Order(8), Explicit]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                Markers = new PacketPb.Types.Markers(),
            };

            SendAndCheck(packet, Filename);
        }
    }
}