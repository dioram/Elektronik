using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Protobuf.Data;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class ExtractFromPackageTests
    {
        private readonly Mock<ICSConverter> _mockedConverter = new();

        delegate void Convert2Callback(ref Vector3 v, ref Quaternion q);

        delegate void Convert1Callback(ref Vector3 v);

        [SetUp]
        public void Setup()
        {
            _mockedConverter.Setup(c => c.Convert(ref It.Ref<Vector3>.IsAny, ref It.Ref<Quaternion>.IsAny))
                    .Callback(new Convert2Callback((ref Vector3 v, ref Quaternion q) =>
                    {
                        v.x = -v.x;
                        q.w = -q.w;
                    }));
            _mockedConverter.Setup(c => c.Convert(ref It.Ref<Vector3>.IsAny))
                    .Callback(new Convert1Callback(((ref Vector3 v) => v.x = -v.x)));
            _mockedConverter.Setup(c => c.Convert(It.IsAny<Vector3>()))
                    .Returns<Vector3>(v => new Vector3(-v.x, v.y, v.z));
            _mockedConverter.Setup(c => c.Convert(It.IsAny<Quaternion>()))
                    .Returns<Quaternion>(q => new Quaternion(q.x, q.y, q.z, -q.w));
            _mockedConverter.Setup(c => c.Convert(It.IsAny<Vector3>(), It.IsAny<Quaternion>()))
                    .Returns((Vector3 v, Quaternion q) => (new Vector3(-v.x, v.y, v.z), new Quaternion(q.x, q.y, q.z, -q.w)));
        }

        [Test]
        public void ExtractPointsTest()
        {
            var packet = new PacketPb
            {
                Points = new PacketPb.Types.Points()
            };
            packet.Points.Data.Add(new PointPb
            {
                Id = 1, 
                Position = new Vector3Pb{X = 2, Y = 3, Z = 4},
                Color = new ColorPb{B = 255, G = 255, R = 255},
                Message = "message"
            });
            var points = packet.ExtractPoints(_mockedConverter.Object).ToArray();
            Assert.AreEqual(1, points.Count());
            Assert.AreEqual(1, points[0].Id);
            Assert.AreEqual(new Vector3(-2, 3, 4), points[0].Position);
            Assert.AreEqual(Color.white, points[0].Color);
            Assert.AreEqual("message", points[0].Message);
        }

        [Test]
        public void ExtractObservationsTest()
        {
            var packet = new PacketPb
            {
                Observations = new PacketPb.Types.Observations()
            };
            packet.Observations.Data.Add(new ObservationPb
            {
                Filename = "1.png",
                Message = "message",
                Orientation = new Vector4Pb{X = 5, Y = 6, Z = 7, W = 8},
                Point = new PointPb
                {
                    Id = 1, 
                    Position = new Vector3Pb{X = 2, Y = 3, Z = 4},
                    Color = new ColorPb{B = 255, G = 255, R = 255},
                    Message = "message"
                }
            });
            var observations = packet.ExtractObservations(_mockedConverter.Object, @"./").ToArray();
            Assert.AreEqual(1, observations.Count());
            Assert.AreEqual(1, observations[0].Id);
            Assert.AreEqual(new Vector3(-2, 3, 4), observations[0].Position);
            Assert.AreEqual(Color.white, observations[0].Color);
            Assert.AreEqual("message", observations[0].Message);
            Assert.AreEqual(new Quaternion(5, 6, 7, -8), observations[0].Rotation);
            Assert.AreEqual(@"./1.png", observations[0].FileName);
        }

        [Test]
        public void ExtractTrackedObjectsTest()
        {
            var packet = new PacketPb
            {
                TrackedObjs = new PacketPb.Types.TrackedObjs()
            };
            packet.TrackedObjs.Data.Add(new TrackedObjPb()
            {
                Id = 1,
                Message = "message",
                Orientation = new Vector4Pb{X = 5, Y = 6, Z = 7, W = 8},
                Position = new Vector3Pb{X = 2, Y = 3, Z = 4},
                Color = new ColorPb{B = 255, G = 255, R = 255},
            });
            var trackedObjects = packet.ExtractTrackedObjects(_mockedConverter.Object).ToArray();
            Assert.AreEqual(1, trackedObjects.Count());
            Assert.AreEqual(1, trackedObjects[0].Id);
            Assert.AreEqual(new Vector3(-2, 3, 4), trackedObjects[0].Position);
            Assert.AreEqual(Color.white, trackedObjects[0].Color);
            Assert.AreEqual("message", trackedObjects[0].Message);
            Assert.AreEqual(new Quaternion(5, 6, 7, -8), trackedObjects[0].Rotation);
        }

        [Test]
        public void ExtractLinesTest()
        {
            var packet = new PacketPb
            {
                Lines = new PacketPb.Types.Lines()
            };
            packet.Lines.Data.Add(new LinePb
            {
                Pt1 = new PointPb
                {
                    Id = 1, 
                    Position = new Vector3Pb{X = 2, Y = 3, Z = 4},
                    Color = new ColorPb{B = 255, G = 255, R = 255},
                    Message = "message"
                },
                Pt2 = new PointPb
                {
                    Id = 1, 
                    Position = new Vector3Pb{X = 5, Y = 6, Z = 7},
                    Color = new ColorPb{B = 0, G = 0, R = 0},
                    Message = "message"
                }
            });
            var lines = packet.ExtractLines(_mockedConverter.Object).ToArray();
            Assert.AreEqual(1, lines.Count());
            Assert.AreEqual(new Vector3(-2, 3, 4), lines[0].Point1.Position);
            Assert.AreEqual(new Vector3(-5, 6, 7), lines[0].Point2.Position);
            Assert.AreEqual(Color.white, lines[0].Point1.Color);
            Assert.AreEqual(Color.black, lines[0].Point2.Color);
        }

        [Test]
        public void ExtractPlanesTest()
        {
            var packet = new PacketPb
            {
                Planes = new PacketPb.Types.Planes()
            };
            packet.Planes.Data.Add(new PlanePb
            {
                Id = 1, 
                Offset = new Vector3Pb{X = 2, Y = 3, Z = 4},
                Normal = new Vector3Pb{X = 5, Y = 6, Z = 7},
                Color = new ColorPb{B = 255, G = 255, R = 255},
                Message = "message"
            });
            var planes = packet.ExtractPlanes(_mockedConverter.Object).ToArray();
            Assert.AreEqual(1, planes.Count());
            Assert.AreEqual(1, planes[0].Id);
            Assert.AreEqual(new Vector3(-2, 3, 4), planes[0].Offset);
            Assert.AreEqual(new Vector3(-5, 6, 7), planes[0].Normal);
            Assert.AreEqual(Color.white, planes[0].Color);
            Assert.AreEqual("message", planes[0].Message);
        }
    }
}