using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Elektronik.Protobuf.Data;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class ShapedTests : TestsBase
    {
        private PointPb[] GeneratePlaneOfPoints(int amount, float side, float noise = 0)
        {
            var rand = new Random();
            return Enumerable.Range(0, amount).Select(id => new PointPb()
            {
                Id = id,
                Position = new Vector3Pb
                {
                    X = rand.NextDouble() * side - side / 2,
                    Y = rand.NextDouble() * side - side / 2,
                    Z = rand.NextDouble() * noise - noise / 2,
                },
            }).ToArray();
        }

        private (PacketPb points, PacketPb observations) GenerateCube(PointPb[] plane, float side)
        {
            var faces = new List<IEnumerable<PointPb>>
            {
                plane.Select(s => new PointPb
                {
                    Id = s.Id + plane.Length * 0, Color = s.Color,
                    Position = new Vector3Pb {X = s.Position.X, Y = s.Position.Y, Z = -side / 2 + s.Position.Z}
                }),
                plane.Select(s => new PointPb
                {
                    Id = s.Id + plane.Length * 1, Color = s.Color,
                    Position = new Vector3Pb {X = s.Position.X, Y = s.Position.Y, Z = side / 2 + s.Position.Z}
                }),
                plane.Select(s => new PointPb
                {
                    Id = s.Id + plane.Length * 2, Color = s.Color,
                    Position = new Vector3Pb {X = s.Position.X, Y = -side / 2 + s.Position.Z, Z = s.Position.Y}
                }),
                plane.Select(s => new PointPb
                {
                    Id = s.Id + plane.Length * 3, Color = s.Color,
                    Position = new Vector3Pb {X = s.Position.X, Y = side / 2 + s.Position.Z, Z = s.Position.Y}
                }),
                plane.Select(s => new PointPb
                {
                    Id = s.Id + plane.Length * 4, Color = s.Color,
                    Position = new Vector3Pb {X = -side / 2 + s.Position.Z, Y = s.Position.Y, Z = s.Position.X}
                }),
                plane.Select(s => new PointPb
                {
                    Id = s.Id + plane.Length * 5, Color = s.Color,
                    Position = new Vector3Pb {X = side / 2 + s.Position.Z, Y = s.Position.Y, Z = s.Position.X}
                }),
            };
            var allPoints = faces.SelectMany(p => p).ToArray();
            
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.Add(allPoints);
            
            var cameraPoses = new []
            {
                new Vector3Pb {X = 0, Y = 0, Z = -side},
                new Vector3Pb {X = 0, Y = 0, Z = side},
                new Vector3Pb {X = 0, Y = -side, Z = 0},
                new Vector3Pb {X = 0, Y = side, Z = 0},
                new Vector3Pb {X = -side, Y = 0, Z = 0},
                new Vector3Pb {X = side, Y = 0, Z = 0},
            };
            var cameraColors = new []
            {
                new ColorPb {R = 0, G = 0, B = 127},
                new ColorPb {R = 0, G = 0, B = 255},
                new ColorPb {R = 0, G = 127, B = 0},
                new ColorPb {R = 0, G = 255, B = 0},
                new ColorPb {R = 127, G = 0, B = 0},
                new ColorPb {R = 255, G = 0, B = 0},
            };
            var cameraOrientations = new[]
            {
                Quaternion.CreateFromYawPitchRoll(0, 0, 0),
                Quaternion.CreateFromYawPitchRoll((float) Math.PI, 0, 0),
                Quaternion.CreateFromYawPitchRoll(0, -(float) Math.PI / 2, 0),
                Quaternion.CreateFromYawPitchRoll(0, (float) Math.PI / 2, 0),
                Quaternion.CreateFromYawPitchRoll((float) Math.PI / 2, 0, 0),
                Quaternion.CreateFromYawPitchRoll(3 * (float) Math.PI / 2, 0, 0),
            };
            var obsPacket = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
            };
            for (int i = 0; i < cameraPoses.Length; i++)
            {
                obsPacket.Observations.Data.Add(new ObservationPb
                {
                    Point = new PointPb {Id = 6 * plane.Length + i, Position = cameraPoses[i], Color = cameraColors[i]},
                    Orientation = new Vector4Pb
                    {
                        X = cameraOrientations[i].X,
                        Y = cameraOrientations[i].Y,
                        Z = cameraOrientations[i].Z,
                        W = cameraOrientations[i].W,
                    },
                    ObservedPoints = {Enumerable.Range(i * plane.Length, plane.Length)},
                });
            }

            return (packet, obsPacket);
        }

        private PacketPb GenerateAdditionalObservations(int pointsAmount, float side)
        {
            var cameraPoses = new []
            {
                new Vector3Pb {X = side, Y = 0, Z = side},
                new Vector3Pb {X = side, Y = 0, Z = -side},
                new Vector3Pb {X = side, Y = side, Z = 0},
                new Vector3Pb {X = side, Y = -side, Z = 0},
                
                new Vector3Pb {X = -side, Y = 0, Z = side},
                new Vector3Pb {X = -side, Y = 0, Z = -side},
                new Vector3Pb {X = -side, Y = side, Z = 0},
                new Vector3Pb {X = -side, Y = -side, Z = 0},
                
                new Vector3Pb {X = 0, Y = side, Z = side},
                new Vector3Pb {X = 0, Y = side, Z = -side},
                new Vector3Pb {X = 0, Y = -side, Z = side},
                new Vector3Pb {X = 0, Y = -side, Z = -side},
            };
            var cameraColors = new []
            {
                new ColorPb{R = 255, G = 0, B = 255},
                new ColorPb{R = 255, G = 0, B = 64},
                new ColorPb{R = 255, G = 255, B = 0},
                new ColorPb{R = 255, G = 64, B = 0},
                
                new ColorPb{R = 64, G = 0, B = 255},
                new ColorPb{R = 64, G = 0, B = 64},
                new ColorPb{R = 64, G = 255, B = 0},
                new ColorPb{R = 64, G = 64, B = 0},
                
                new ColorPb{R = 0, G = 255, B = 255},
                new ColorPb{R = 0, G = 255, B = 64},
                new ColorPb{R = 0, G = 64, B = 255},
                new ColorPb{R = 0, G = 64, B = 64},
            };
            var cameraOrientations = new[]
            {
                Quaternion.CreateFromYawPitchRoll(-3 * (float) Math.PI / 4, 0, 0),
                Quaternion.CreateFromYawPitchRoll(-(float) Math.PI / 4, 0, 0),
                Quaternion.CreateFromYawPitchRoll(-(float) Math.PI / 2, (float) Math.PI / 4, 0),
                Quaternion.CreateFromYawPitchRoll(-(float) Math.PI / 2, -(float) Math.PI / 4, 0),
                
                Quaternion.CreateFromYawPitchRoll(3 * (float) Math.PI / 4, 0, 0),
                Quaternion.CreateFromYawPitchRoll((float) Math.PI / 4, 0, 0),
                Quaternion.CreateFromYawPitchRoll((float) Math.PI / 2, (float) Math.PI / 4, 0),
                Quaternion.CreateFromYawPitchRoll((float) Math.PI / 2, -(float) Math.PI / 4, 0),
                
                Quaternion.CreateFromYawPitchRoll(0, 3 * (float) Math.PI / 4, 0),
                Quaternion.CreateFromYawPitchRoll(0, (float) Math.PI / 4, 0),
                Quaternion.CreateFromYawPitchRoll(0, -3 * (float) Math.PI / 4, 0),
                Quaternion.CreateFromYawPitchRoll(0, -(float) Math.PI / 4, 0),
            };

            var points = new[]
            {
                new[] {2, 6},
                new[] {1, 6},
                new[] {4, 6},
                new[] {3, 6},

                new[] {2, 5},
                new[] {1, 5},
                new[] {4, 5},
                new[] {3, 5},

                new[] {2, 4},
                new[] {1, 4},
                new[] {2, 3,},
                new[] {1, 3,},
            };
            
            var obsPacket = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
            };
            for (int i = 0; i < cameraPoses.Length; i++)
            {
                obsPacket.Observations.Data.Add(new ObservationPb
                {
                    Point = new PointPb {Id = -i, Position = cameraPoses[i], Color = cameraColors[i]},
                    Orientation = new Vector4Pb
                    {
                        X = cameraOrientations[i].X,
                        Y = cameraOrientations[i].Y,
                        Z = cameraOrientations[i].Z,
                        W = cameraOrientations[i].W,
                    },
                    ObservedPoints = { points[i].SelectMany(p => Enumerable.Range(p * pointsAmount, pointsAmount))},
                });
            }

            return obsPacket;
        }
        
        [Test, Explicit]
        public void Cube()
        {
            var amount = 200;
            var side = 2f;
            var defaultPlane = GeneratePlaneOfPoints(amount, side);
            var (packet, obsPacket) = GenerateCube(defaultPlane, side);

            SendAndCheck(packet);
            Thread.Sleep(200);
            SendAndCheck(obsPacket);
            Thread.Sleep(200);
            SendAndCheck(GenerateAdditionalObservations(amount, side));
        }

        [Test, Explicit]
        public void NoisedCube()
        {
            var amount = 1000;
            var side = 2f;
            var defaultPlane = GeneratePlaneOfPoints(amount, side, 0.1f);
            var (packet, obsPacket) = GenerateCube(defaultPlane, side);


            SendAndCheck(packet);
            Thread.Sleep(200);
            SendAndCheck(obsPacket);
        }
    }
}