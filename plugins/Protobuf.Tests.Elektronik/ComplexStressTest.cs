using System;
using System.Linq;
using System.Threading;
using Elektronik.Protobuf.Data;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class ComplexStressTest : TestsBase
    {
        private PointPb[] GeneratePoints(int amount, float scale)
        {
            var rand = new Random();
            return Enumerable.Range(0, amount).Select(id => new PointPb()
            {
                Id = id,
                Message = $"{id}",
                Position = new Vector3Pb
                {
                    X = rand.NextDouble() * scale,
                    Y = rand.NextDouble() * scale,
                    Z = rand.NextDouble() * scale,
                },
                Color = new ColorPb {B = rand.Next(255), G = rand.Next(255), R = rand.Next(255)},
            }).ToArray();
        }

        private ObservationPb[] GenerateObservations(int amount, float distance)
        {
            var rand = new Random();
            return Enumerable.Range(0, amount).Select(id => new ObservationPb()
            {
                Filename = "",
                Orientation = new Vector4Pb { Y = 0.70710676908493042, W = 0.70710676908493042 },
                Point = new PointPb()
                {
                    Id = id,
                    Message = $"{id}",
                    Position = new Vector3Pb
                    {
                        X = 0,
                        Y = 0,
                        Z = id * distance / amount,
                    },
                    Color = new ColorPb {B = rand.Next(255), G = rand.Next(255), R = rand.Next(255)},
                }
            }).ToArray();
        }

        private void MoveData(PointPb[] points, ObservationPb[] observations, int amountPoints, int amountObs,
                              float distance, bool shift)
        {
            foreach (var point in points)
            {
                point.Id += amountPoints;
                if (shift)
                {
                    point.Position.Z %= distance;
                    point.Position.X += distance;
                }
                else
                {
                    point.Position.Z += distance;
                }
            }

            foreach (var observation in observations)
            {
                observation.Point.Id += amountObs;
                if (shift)
                {
                    observation.Point.Position.Z %= distance;
                    observation.Point.Position.X += distance;
                }
                else
                {
                    observation.Point.Position.Z += distance;
                }
            }
        }

        [Test, Explicit]
        public void ComplexTest()
        {
            int iterations = 1000;
            int pointsAmount = 600;
            int obsAmount = 5;
            float scale = 1f;
            
            var trackedPacket = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            trackedPacket.TrackedObjs.Data.Add(new[] {new TrackedObjPb {Id = 1, Orientation = new Vector4Pb {W = 1}}});
            var response = MapClient.Handle(trackedPacket);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);

            var points = GeneratePoints(pointsAmount, scale);
            var observations = GenerateObservations(obsAmount, scale);

            for (var i = 0; i < iterations; i++)
            {
                var pointsPacket = new PacketPb
                {
                    Action = PacketPb.Types.ActionType.Add,
                    Points = new PacketPb.Types.Points(),
                    Special = true,
                };
                pointsPacket.Points.Data.Add(points);


                var obsPacket = new PacketPb
                {
                    Action = PacketPb.Types.ActionType.Add,
                    Observations = new PacketPb.Types.Observations()
                };
                obsPacket.Observations.Data.Add(observations);

                trackedPacket = new PacketPb
                {
                    Action = PacketPb.Types.ActionType.Update,
                    TrackedObjs = new PacketPb.Types.TrackedObjs(),
                };
                trackedPacket.TrackedObjs.Data.Add(new[]
                {
                    new TrackedObjPb
                    {
                        Id = 1,
                        Position = observations.Last().Point.Position,
                        Orientation = observations.Last().Orientation
                    }
                });
                
                SendAndCheck(pointsPacket);
                SendAndCheck(obsPacket);
                SendAndCheck(trackedPacket);
                
                Thread.Sleep(25);
                MoveData(points, observations, pointsAmount, obsAmount, scale, (i+1) % 100 == 0);
            }
        }
    }
}