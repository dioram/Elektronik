using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class ExtractFromPackageTests
    {
        [Test]
        public void ExtractPointsTest()
        {
            var packet = new PacketPb { Points = new PacketPb.Types.Points() };
            packet.Points.Data.Add(new[]
            {
                new PointPb
                {
                    Id = 1,
                    Position = new Vector3Pb { X = 2, Y = 3, Z = 4 },
                    Color = new ColorPb { B = 255, G = 255, R = 255 },
                    Message = "message"
                },
                new PointPb
                {
                    Id = 4,
                    Position = new Vector3Pb { X = 5, Y = 6, Z = 7 },
                    Color = new ColorPb { B = 0, G = 0, R = 0 },
                    Message = "test"
                }
            });
            var expected = new[]
            {
                new SlamPointDiff(1, new Vector3(2, -3, 4), Color.white, "message"),
                new SlamPointDiff(4, new Vector3(5, -6, 7), Color.black, "test"),
            };

            var points = packet.ExtractPoints(new ProtobufToUnityConverter());

            points.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ExtractObservationsTest()
        {
            var packet = new PacketPb { Observations = new PacketPb.Types.Observations() };
            packet.Observations.Data.Add(new[]
            {
                new ObservationPb
                {
                    Filename = "1.png",
                    Message = "message",
                    Orientation = new Vector4Pb { X = 5, Y = 6, Z = 7, W = 8 },
                    ObservedPoints = { 0, 1, 3, 5 },
                    Point = new PointPb
                    {
                        Id = 1,
                        Position = new Vector3Pb { X = 2, Y = 3, Z = 4 },
                        Color = new ColorPb { B = 255, G = 255, R = 255 },
                        Message = "message"
                    }
                },
                new ObservationPb
                {
                    Filename = "3.png",
                    Message = "test",
                    Orientation = new Vector4Pb { X = 8, Y = 7, Z = 6, W = 5 },
                    ObservedPoints = { 3, 2, 1, },
                    Point = new PointPb
                    {
                        Id = 4,
                        Position = new Vector3Pb { X = 5, Y = 6, Z = 7 },
                        Color = new ColorPb { B = 0, G = 0, R = 0 },
                        Message = "test"
                    }
                },
            });
            var expected = new[]
            {
                new SlamObservationDiff(1, new Vector3(2, -3, 4), Color.white,
                                        new Quaternion(5, -6, 7, -8), new[] { 0, 1, 3, 5 }, "message", @"./1.png"),
                new SlamObservationDiff(4, new Vector3(5, -6, 7), Color.black,
                                        new Quaternion(8, -7, 6, -5), new[] { 3, 2, 1 }, "test", @"./3.png")
            };

            var observations = packet.ExtractObservations(new ProtobufToUnityConverter(), @"./");

            observations.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ExtractTrackedObjectsTest()
        {
            var packet = new PacketPb { TrackedObjs = new PacketPb.Types.TrackedObjs() };
            packet.TrackedObjs.Data.Add(new[]
            {
                new TrackedObjPb
                {
                    Id = 1,
                    Message = "message",
                    Orientation = new Vector4Pb { X = 5, Y = 6, Z = 7, W = 8 },
                    Position = new Vector3Pb { X = 2, Y = 3, Z = 4 },
                    Color = new ColorPb { B = 255, G = 255, R = 255 },
                },
                new TrackedObjPb
                {
                    Id = 3,
                    Message = "test",
                    Orientation = new Vector4Pb { X = 8, Y = 7, Z = 6, W = 5 },
                    Position = new Vector3Pb { X = 5, Y = 6, Z = 7 },
                    Color = new ColorPb { B = 0, G = 0, R = 0 },
                },
            });
            var expected = new[]
            {
                new SlamTrackedObjectDiff(1, new Vector3(2, -3, 4), new Quaternion(5, -6, 7, -8), Color.white,
                                          "message"),
                new SlamTrackedObjectDiff(3, new Vector3(5, -6, 7), new Quaternion(8, -7, 6, -5), Color.black,
                                          "test"),
            };

            var trackedObjects = packet.ExtractTrackedObjects(new ProtobufToUnityConverter());

            trackedObjects.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ExtractLinesTest()
        {
            var packet = new PacketPb { Lines = new PacketPb.Types.Lines() };
            packet.Lines.Data.Add(new[]
            {
                new LinePb
                {
                    Pt1 = new PointPb
                    {
                        Id = 1,
                        Position = new Vector3Pb { X = 2, Y = 3, Z = 4 },
                        Color = new ColorPb { B = 255, G = 255, R = 255 },
                        Message = "message"
                    },
                    Pt2 = new PointPb
                    {
                        Id = 3,
                        Position = new Vector3Pb { X = 5, Y = 6, Z = 7 },
                        Color = new ColorPb { B = 0, G = 0, R = 0 },
                        Message = "message"
                    }
                },
                new LinePb
                {
                    Pt1 = new PointPb
                    {
                        Id = 5,
                        Position = new Vector3Pb { X = 0, Y = 1, Z = 5 },
                        Color = new ColorPb { B = 255, G = 0, R = 0 },
                        Message = "test"
                    },
                    Pt2 = new PointPb
                    {
                        Id = 6,
                        Position = new Vector3Pb { X = 3, Y = 5, Z = 9 },
                        Color = new ColorPb { B = 0, G = 255, R = 0 },
                        Message = "test"
                    }
                }
            });
            var expected = new[]
            {
                new SlamLineDiff
                {
                    Point1 = new SlamPointDiff(1, new Vector3(2, -3, 4), Color.white, "message"),
                    Point2 = new SlamPointDiff(3, new Vector3(5, -6, 7), Color.black, "message"),
                },
                new SlamLineDiff
                {
                    Point1 = new SlamPointDiff(5, new Vector3(0, -1, 5), Color.blue, "test"),
                    Point2 = new SlamPointDiff(6, new Vector3(3, -5, 9), Color.green, "test"),
                }
            };

            var lines = packet.ExtractLines(new ProtobufToUnityConverter());

            lines.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ExtractPlanesTest()
        {
            var packet = new PacketPb { Planes = new PacketPb.Types.Planes() };
            packet.Planes.Data.Add(new[]
            {
                new PlanePb
                {
                    Id = 1,
                    Offset = new Vector3Pb { X = 2, Y = 3, Z = 4 },
                    Normal = new Vector3Pb { X = 5, Y = 6, Z = 7 },
                    Color = new ColorPb { B = 255, G = 255, R = 255 },
                    Message = "message"
                },
                new PlanePb
                {
                    Id = 3,
                    Offset = new Vector3Pb { X = 0, Y = 1, Z = 3 },
                    Normal = new Vector3Pb { X = 5, Y = 7, Z = 8 },
                    Color = new ColorPb { B = 0, G = 0, R = 0 },
                    Message = "test"
                }
            });
            var expected = new[]
            {
                new SlamPlaneDiff(1, new Vector3(2, -3, 4), new Vector3(5, -6, 7), Color.white, "message"),
                new SlamPlaneDiff(3, new Vector3(0, -1, 3), new Vector3(5, -7, 8), Color.black, "test")
            };

            var planes = packet.ExtractPlanes(new ProtobufToUnityConverter());

            planes.Should().BeEquivalentTo(expected);
        }
    }
}