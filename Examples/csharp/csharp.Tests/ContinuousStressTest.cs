using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Elektronik.Common.Data.Pb;
using NUnit.Framework;

namespace csharp.Tests
{
    public class ContinuousStressTest : TestsBase
    {
        private List<PointPb[]> movie;
        private List<PacketPb.Types.ActionType> commands;

        private Vector3Pb[] centers = new[]
        {
                new Vector3Pb {X = 0, Y = 0, Z = 0,},

                new Vector3Pb {X = 0, Y = 2, Z = 0,},
                new Vector3Pb {X = 0, Y = 2, Z = 2,},
                new Vector3Pb {X = 0, Y = 2, Z = -2,},
                new Vector3Pb {X = 2, Y = 2, Z = 0,},
                new Vector3Pb {X = 2, Y = 2, Z = 2,},
                new Vector3Pb {X = 2, Y = 2, Z = -2,},
                new Vector3Pb {X = -2, Y = 2, Z = 0,},
                new Vector3Pb {X = -2, Y = 2, Z = 2,},
                new Vector3Pb {X = -2, Y = 2, Z = -2,},

                new Vector3Pb {X = 0, Y = -2, Z = 0,},
                new Vector3Pb {X = 0, Y = -2, Z = 2,},
                new Vector3Pb {X = 0, Y = -2, Z = -2,},
                new Vector3Pb {X = 2, Y = -2, Z = 0,},
                new Vector3Pb {X = 2, Y = -2, Z = 2,},
                new Vector3Pb {X = 2, Y = -2, Z = -2,},
                new Vector3Pb {X = -2, Y = -2, Z = 0,},
                new Vector3Pb {X = -2, Y = -2, Z = 2,},
                new Vector3Pb {X = -2, Y = -2, Z = -2,},

                new Vector3Pb {X = 0, Y = 0, Z = 2,},
                new Vector3Pb {X = 0, Y = 0, Z = -2,},
                new Vector3Pb {X = 2, Y = 0, Z = 0,},
                new Vector3Pb {X = 2, Y = 0, Z = 2,},
                new Vector3Pb {X = 2, Y = 0, Z = -2,},
                new Vector3Pb {X = -2, Y = 0, Z = 0,},
                new Vector3Pb {X = -2, Y = 0, Z = 2,},
                new Vector3Pb {X = -2, Y = 0, Z = -2,},
        };

        public ContinuousStressTest()
        {
            MovieGenerator.CreateMovie(3, out movie, out commands);
        }

        [Test]
        public void CycleTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                var packet = new PacketPb()
                {
                        Special = true,
                        Action = commands[i % commands.Count],
                        Points = new PacketPb.Types.Points(),
                };
                if (commands[i % commands.Count] != PacketPb.Types.ActionType.Clear)
                {
                    packet.Points.Data.Add(movie[i % commands.Count]);
                }

                TestContext.WriteLine($"Command: {commands[i % commands.Count]}");
                var response = m_mapClient.Handle(packet);
                Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
                Thread.Sleep(250);
            }
        }

        [Test]
        public void OverflowTest()
        {
            int i = 0;
            int defaultPacketSize = 50000;
            int resolution = 10;
            var rand = new Random();
            var center = new Vector3Pb {X = 0, Y = 0, Z = 0};
            while (true)
            {
                int packetSize = rand.Next(defaultPacketSize / 2, defaultPacketSize + defaultPacketSize / 2);
                var packet = new PacketPb()
                {
                        Special = true,
                        Action = PacketPb.Types.ActionType.Add,
                        Points = new PacketPb.Types.Points(),
                };
                packet.Points.Data.Add(MovieGenerator.CreatePoints(packetSize, center, resolution, out PointPb[] _, i));
                i += packetSize;
                var response = m_mapClient.Handle(packet);
                Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
                Thread.Sleep(500);
            }
        }

        [Test]
        public void SpreadedBlocksOverflowTest()
        {
            for (var index = 0; index < centers.Length; index++)
            {
                var c = centers[index];
                int packetSize = 65536;
                int resolution = 100;
                var center = new Vector3Pb {X = c.X * resolution, Y = c.Y * resolution, Z = c.Z * resolution,};
                for (int i = 0; i < 1024 * 1024; i += packetSize)
                {
                    var packet = new PacketPb()
                    {
                            Special = true,
                            Action = PacketPb.Types.ActionType.Add,
                            Points = new PacketPb.Types.Points(),
                    };
                    packet.Points.Data.Add(MovieGenerator.CreatePoints(packetSize,
                                                                       center,
                                                                       resolution,
                                                                       out PointPb[] _,
                                                                       index * 1024 * 1024 + i));
                    var response = m_mapClient.Handle(packet);
                    Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
                    Thread.Sleep(250);
                }
            }
        }
    }


    public static class MovieGenerator
    {
        static readonly float[,] m = new float[,] {{0.8f, 0.01f}, {0.01f, 0.8f}};

        public static void CreateMovie(float resolution, out List<PointPb[]> points,
                                       out List<PacketPb.Types.ActionType> commands)
        {
            points = new List<PointPb[]>();
            commands = new List<PacketPb.Types.ActionType>();
            var center = new Vector3Pb {X = 0, Y = 0, Z = 0};

            PointPb[] allPoints;

            points.Add(CreatePoints(100000, center, resolution, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Add);

            points.Add(AddPoints(100000, center, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Add);

            points.Add(RemovePoints(60000, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Remove);

            points.Add(AddPoints(30000, center, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Add);

            points.Add(UpdatePoints(80000, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Update);

            points.Add(AddPoints(90000, center, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Add);

            points.Add(RemovePoints(90000, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Remove);

            points.Add(AddPoints(90000, center, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Add);

            points.Add(UpdatePoints(60000, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Update);

            points.Add(UpdatePoints(20000, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Update);

            points.Add(UpdatePoints(100000, resolution, allPoints, out allPoints));
            commands.Add(PacketPb.Types.ActionType.Update);

            points.Add(Clear());
            commands.Add(PacketPb.Types.ActionType.Clear);
        }


        public static PointPb[] CreatePoints(int amount, Vector3Pb center, float resolution, out PointPb[] after,
                                             int minId = 0)
        {
            var result = new PointPb[amount];
            var rand = new Random();
            for (int i = 0; i < amount; i++)
            {
                result[i] = CreatePoint(i + minId, center, resolution);
            }

            after = result;
            return result;
        }

        static PointPb[] AddPoints(int amount, Vector3Pb center, float resolution, PointPb[] before,
                                   out PointPb[] after)
        {
            var newPoints = CreatePoints(amount, center, resolution, out PointPb[] _, before.Last().Id + 1);
            after = new PointPb[before.Length + newPoints.Length];
            Array.Copy(before, 0, after, 0, before.Length);
            Array.Copy(newPoints, 0, after, before.Length, newPoints.Length);
            return newPoints;
        }

        static PointPb[] UpdatePoints(int amount, float resolution, PointPb[] before, out PointPb[] after)
        {
            var rand = new Random();
            var result = new List<PointPb>();
            after = new PointPb[before.Length];
            before.CopyTo(after, 0);

            HashSet<int> indexes2update = new HashSet<int>();
            for (int i = 0; i < amount; i++)
            {
                indexes2update.Add(rand.Next(before.Length));
            }

            for (int i = 0; i < after.Length; i++)
            {
                if (indexes2update.Contains(after[i].Id))
                {
                    var newPoint = CreatePoint(after[i].Id, after[i].Position, resolution / 10);
                    after[i] = newPoint;
                    result.Add(newPoint);
                }
            }

            return result.ToArray();
        }

        static PointPb[] RemovePoints(int amount, PointPb[] before, out PointPb[] after)
        {
            var rand = new Random();
            HashSet<int> indexes2remove = new HashSet<int>();
            for (int i = 0; i < amount; i++)
            {
                indexes2remove.Add(rand.Next(before.Length));
            }

            after = before.Where((p, i) => !indexes2remove.Contains(i)).ToArray();
            return before.Where((p, i) => indexes2remove.Contains(i)).ToArray();
        }

        static PointPb[] Clear()
        {
            return new PointPb[0];
        }

        static PointPb CreatePoint(int id, Vector3Pb center, float maxDistance)
        {
            var rand = new Random();
            var pos = new Vector3Pb
            {
                    X = -maxDistance + rand.NextDouble() * maxDistance * 2 + center.X,
                    Y = -maxDistance + rand.NextDouble() * maxDistance * 2 + center.Y,
                    Z = -maxDistance + rand.NextDouble() * maxDistance * 2 + center.Z,
            };
            ColorPb color;
            if (Math.Abs(pos.Z / maxDistance) < 0.1)
                color = new ColorPb {R = 0, G = 0, B = 0};
            else if (Math.Abs(pos.Z / maxDistance) < 0.2)
                color = new ColorPb {R = 255, G = 0, B = 0};
            else if (Math.Abs(pos.Z / maxDistance) < 0.3)
                color = new ColorPb {R = 0, G = 255, B = 0};
            else if (Math.Abs(pos.Z / maxDistance) < 0.45)
                color = new ColorPb {R = 0, G = 0, B = 255};
            else if (Math.Abs(pos.Z / maxDistance) < 0.55)
                color = new ColorPb {R = 255, G = 255, B = 0};
            else if (Math.Abs(pos.Z / maxDistance) < 0.65)
                color = new ColorPb {R = 0, G = 255, B = 255};
            else
                color = new ColorPb {R = 255, G = 0, B = 255};

            return new PointPb {Id = id, Position = pos, Color = color};
        }
    }
}