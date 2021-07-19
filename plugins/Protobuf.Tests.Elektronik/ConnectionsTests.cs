using Elektronik.Protobuf.Data;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class ConnectionsTests : TestsBase
    {
        private static readonly string Filename = $"{nameof(ConnectionsTests)}.dat";

        [Test, Order(1), Explicit]
        public void CreatePoints()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
                Special = true,
            };

            packet.Points.Data.Add(new PointPb[]
            {
                new() {Id = 0, Position = new Vector3Pb {X = -2f, Z = 1f}, Color = new ColorPb {R = 255}},
                new() {Id = 1, Position = new Vector3Pb {X = -1f, Z = 1f}, Color = new ColorPb {R = 255}},
                new() {Id = 2, Position = new Vector3Pb {X = 0f, Z = 1f}, Color = new ColorPb {R = 255}},
                new() {Id = 3, Position = new Vector3Pb {X = 1f, Z = 1f}, Color = new ColorPb {R = 255}},
                new() {Id = 4, Position = new Vector3Pb {X = 2f, Z = 1f}, Color = new ColorPb {R = 255}},
            });

            SendAndCheck(packet, Filename, true);
        }

        [Test, Order(2), Explicit]
        public void CreateObservations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
                Special = true,
            };

            packet.Observations.Data.Add(new ObservationPb[]
            {
                new()
                {
                    Point = new PointPb
                    {
                        Id = 0, Position = new Vector3Pb {X = -1, Y = -1}, Color = new ColorPb {R = 255}
                    },
                    ObservedPoints = {0, 1, 2}
                },
                new()
                {
                    Point = new PointPb
                    {
                        Id = 1, Position = new Vector3Pb {X = 1, Y = -1}, Color = new ColorPb {G = 255}
                    },
                    ObservedPoints = {2}
                },
                new()
                {
                    Point = new PointPb
                    {
                        Id = 2, Position = new Vector3Pb {X = -1, Y = 1}, Color = new ColorPb {B = 255}
                    },
                    ObservedPoints = {3, 4}
                },
                new()
                {
                    Point = new PointPb
                    {
                        Id = 3, Position = new Vector3Pb {X = 1, Y = 1}, Color = new ColorPb {R = 255, B = 255}
                    },
                    ObservedPoints = {0, 4}
                },
            });

            // Connections:
            // 0 - 1 (R - G)
            // 0 - 3 (R - RB)
            // 2 - 3 (B - RB)
            SendAndCheck(packet, Filename);
        }

        [Test, Order(3), Explicit]
        public void UpdateObservations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Special = true,
            };

            packet.Observations.Data.Add(new ObservationPb[]
            {
                new() {Point = new PointPb {Id = 0}, ObservedPoints = {0, 1, 2, 3}},
                new() {Point = new PointPb {Id = 1}, ObservedPoints = {2, 4}},
                new() {Point = new PointPb {Id = 2}, ObservedPoints = {3, 4}},
                new() {Point = new PointPb {Id = 3}, ObservedPoints = { }}, // {0, 4}
            });

            // Connections:
            // 0 - 1 (R - G)
            // 0 - 2 (R - B)
            // 0 - 3 (R - RB)
            // 1 - 2 (G - B)
            // 1 - 3 (G - RB)
            // 2 - 3 (B - RB)
            SendAndCheck(packet, Filename);
        }

        [Test, Order(4), Explicit]
        public void AddPoints()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
                Special = true,
            };

            packet.Points.Data.Add(new PointPb[]
            {
                new() {Id = 5, Position = new Vector3Pb {Y = 1f, Z = 1f}, Color = new ColorPb {G = 255}},
                new() {Id = 6, Position = new Vector3Pb {Y = -1f, Z = 1f}, Color = new ColorPb {G = 255}},
            });

            SendAndCheck(packet, Filename);

            var obsPacket = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Special = true,
            };

            obsPacket.Observations.Data.Add(new ObservationPb[]
            {
                new() {Point = new PointPb {Id = 0}, ObservedPoints = {0, 1, 2, 3, 5}},
                new() {Point = new PointPb {Id = 1}, ObservedPoints = {2, 4}},
                new() {Point = new PointPb {Id = 2}, ObservedPoints = {3, 4, 6}},
                new() {Point = new PointPb {Id = 3}, ObservedPoints = {5, 6}},
            });


            // Connections:
            // 0 - 1 (R - G)
            // 0 - 2 (R - B)
            // 0 - 3 (R - RB)
            // 1 - 2 (G - B)
            // 2 - 3 (B - RB)
            SendAndCheck(obsPacket, Filename);
        }

        [Test, Order(5), Explicit]
        public void AddObservations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
                Special = true,
            };

            packet.Observations.Data.Add(new ObservationPb[]
            {
                new()
                {
                    Point = new PointPb
                    {
                        Id = 4, Position = new Vector3Pb {Y = 2}, Color = new ColorPb {R = 255, G = 255}
                    },
                    ObservedPoints = {0, 1, 2, 3, 5}
                },
            });

            // Connections:
            // 0 - 1 (R - G)
            // 0 - 2 (R - B)
            // 0 - 3 (R - RB)
            // 0 - 4 (R - RG}
            // 1 - 2 (G - B)
            // 1 - 4 (G - RG)
            // 2 - 3 (B - RB)
            // 2 - 4 (B - RG)
            // 3 - 4 (RG - RB)
            SendAndCheck(packet, Filename);
        }

        [Test, Order(6), Explicit]
        public void RemoveSomePoints()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Points = new PacketPb.Types.Points(),
                Special = true,
            };

            packet.Points.Data.Add(new PointPb[]
            {
                new() {Id = 2},
                new() {Id = 0},
                new() {Id = 5},
            });
            // Observations:
            // 0: 1, 3
            // 1: 4
            // 2: 3, 4, 6
            // 3: 6
            // 4: 1, 3, 5

            // Connections:
            // 0 - 2 (R - B)
            // 0 - 4 (R - RG}
            // 1 - 2 (G - B)
            // 2 - 3 (B - RB)
            // 2 - 4 (B - RG)
            SendAndCheck(packet, Filename);
        }

        [Test, Order(7), Explicit]
        public void RemoveSomeObservations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Observations = new PacketPb.Types.Observations(),
                Special = true,
            };

            packet.Observations.Data.Add(new ObservationPb[]
            {
                new() {Point = new PointPb {Id = 0}},
            });

            // Observations:
            // 1: 4
            // 2: 3, 4, 6
            // 3: 6
            // 4: 1, 3, 5

            // Connections:
            // 1 - 2 (G - B)
            // 2 - 3 (B - RB)
            // 2 - 4 (B - RG)
            SendAndCheck(packet, Filename);
        }

        [Test, Order(8), Explicit]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                Points = new PacketPb.Types.Points(),
            };
            SendAndCheck(packet, Filename);

            var obsPacket = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                Observations = new PacketPb.Types.Observations(),
            };
            SendAndCheck(obsPacket, Filename);
        }
    }
}