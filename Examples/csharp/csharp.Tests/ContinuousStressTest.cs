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

        public ContinuousStressTest()
        {
            MovieGenerator.CreateMovie(3, out movie, out commands);
        }

        [Test]
        public void Run()
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
                var response = m_mapClient.Handle(packet);
                Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
                Thread.Sleep(250);
            }
        }
    }
    
    public static class MovieGenerator
    {
        static readonly float[,] m = new float[,] {{0.8f, 0.01f}, {0.01f, 0.8f}};

        public static void CreateMovie(float resolution, out List<PointPb[]> points, out List<PacketPb.Types.ActionType> commands)
        {
            points = new List<PointPb[]>();
            commands = new List<PacketPb.Types.ActionType>();

            points.Add(CreatePoints(40000, resolution));
            commands.Add(PacketPb.Types.ActionType.Add);
            
            points.Add(AddPoints(20000, resolution, points.Last()));
            commands.Add(PacketPb.Types.ActionType.Add);
            
            points.Add(RemovePoints(15000, points.Last()));
            commands.Add(PacketPb.Types.ActionType.Remove);
            
            points.Add(AddPoints(5000, resolution, points.Last()));
            commands.Add(PacketPb.Types.ActionType.Add);
            
            points.Add(UpdatePoints(30000, resolution, points.Last()));
            commands.Add(PacketPb.Types.ActionType.Update);
            
            points.Add(AddPoints(40000, resolution, points.Last()));
            commands.Add(PacketPb.Types.ActionType.Add);
            
            points.Add(RemovePoints(40000, points.Last()));
            commands.Add(PacketPb.Types.ActionType.Remove);
            
            points.Add(UpdatePoints(20000, resolution, points.Last()));
            commands.Add(PacketPb.Types.ActionType.Clear);
            
            points.Add(Clear());
        }


        static PointPb[] CreatePoints(int amount, float resolution, int minId = 0)
        {
            var result = new PointPb[amount];
            var rand = new Random();
            for (int i = 0; i < amount; i++)
            {
                var x = -resolution + rand.NextDouble() * resolution * 2;
                var y = -resolution + rand.NextDouble() * resolution * 2;
                result[i] = CreatePoint(x, y, i + minId, resolution);
            }

            return result;
        }

        static PointPb[] AddPoints(int amount, float resolution, PointPb[] before)
        {
            var newPoints = CreatePoints(amount, resolution, before.Last().Id + 1);
            var result = new PointPb[before.Length + newPoints.Length];
            Array.Copy(before, 0, result, 0, before.Length);
            Array.Copy(newPoints, 0, result, before.Length, newPoints.Length);
            return result;
        }

        static PointPb[] UpdatePoints(int amount, float resolution, PointPb[] before)
        {
            var rand = new Random();
            var result = new PointPb[before.Length];
            before.CopyTo(result, 0);

            HashSet<int> indexes2update = new HashSet<int>();
            for (int i = 0; i < amount; i++)
            {
                indexes2update.Add(rand.Next(before.Length));
            }

            for (int i = 0; i < result.Length; i++)
            {
                if (indexes2update.Contains(result[i].Id))
                {
                    result[i] = CreatePoint(result[i].Position.X, result[i].Position.Y, result[i].Id, resolution);
                }
            }

            return result;
        }

        static PointPb[] RemovePoints(int amount, PointPb[] before)
        {
            var rand = new Random();
            HashSet<int> indexes2remove = new HashSet<int>();
            for (int i = 0; i < amount; i++)
            {
                indexes2remove.Add(rand.Next(before.Length));
            }
            return before.Where((p, i) => !indexes2remove.Contains(i)).ToArray();
        }

        static PointPb[] Clear()
        {
            return new PointPb[0];
        }

        static PointPb CreatePoint(double x, double y, int id, float resolution)
        {
            var rand = new Random();
            var r = -resolution + rand.NextDouble() * resolution * 2;
            var pos = new Vector3Pb{X = x, Y = y, Z = r};
            ColorPb color;
            if (Math.Abs(pos.Z / resolution) < 0.1)
                color = new ColorPb{R = 0, G = 0, B = 0};
            else if (Math.Abs(pos.Z / resolution) < 0.2)
                color = new ColorPb{R = 255, G = 0, B = 0};
            else if (Math.Abs(pos.Z / resolution) < 0.3)
                color = new ColorPb{R = 0, G = 255, B = 0};
            else if (Math.Abs(pos.Z / resolution) < 0.45)
                color = new ColorPb{R = 0, G = 0, B = 255};
            else if (Math.Abs(pos.Z / resolution) < 0.55)
                color = new ColorPb{R = 255, G = 255, B = 0};
            else if (Math.Abs(pos.Z / resolution) < 0.65)
                color = new ColorPb{R = 0, G = 255, B = 255};
            else
                color = new ColorPb{R = 255, G = 0, B = 255};

            return new PointPb {Id = id, Position = pos, Color = color};
        }
    }
}