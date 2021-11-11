using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Plugins.Common;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Recorders;
using FluentAssertions;
using Google.Protobuf;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class RecorderTests
    {
        private string _filename;
        private readonly ICSConverter _converter = new RightHandToLeftHandConverter();

        [TearDown]
        public void TearDown()
        {
            File.Delete(_filename);
        }

        [Test]
        public void PointsTest()
        {
            var sut = CreateRecorder(0);
            var points = new[]
            {
                new SlamPoint(0, Vector3.one, Color.blue),
                new SlamPoint(1, Vector3.down, Color.red),
                new SlamPoint(-10, Vector3.forward, Color.gray, "test message")
            };
            var morePoints = new[]
            {
                new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                new SlamPoint(-10, Vector3.up, Color.white, "test message")
            };

            sut.OnItemsAdded(null, new AddedEventArgs<SlamPoint>(points));
            sut.OnItemsUpdated(null, new UpdatedEventArgs<SlamPoint>(morePoints));
            sut.OnItemsRemoved(null, new RemovedEventArgs<SlamPoint>(new List<int> { 0 }));
            sut.Dispose();

            using var input = File.OpenRead(_filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            packet.Action.Should().Be(PacketPb.Types.ActionType.Add);
            CheckPoints(points, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            anotherPacket.Action.Should().Be(PacketPb.Types.ActionType.Update);
            CheckPoints(morePoints, anotherPacket);

            var yetAnotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            yetAnotherPacket.Action.Should().Be(PacketPb.Types.ActionType.Remove);
            var removingPoints = yetAnotherPacket.ExtractPoints(_converter).ToList();
            removingPoints.Count.Should().Be(1);
            removingPoints[0].Id.Should().Be(0);
            CheckMetadata(input, 3);
        }

        [Test]
        public void ObservationsTest()
        {
            var sut = CreateRecorder(1);
            var observations = new[]
            {
                new SlamObservation(0, Vector3.one, Color.blue, Quaternion.identity, "123", "f.jpg"),
                new SlamObservation(1, Vector3.down, Color.red, new Quaternion(0, 5, 15, 3), "321", ""),
                new SlamObservation(-10, Vector3.forward, Color.gray, new Quaternion(59, 46, 3, 24), "adsf", "f.jpg"),
            };
            var moreObservations = new[]
            {
                new SlamObservation(0, Vector3.zero, Color.red, new Quaternion(6, 2, 1, 0), "123", "f.jpg"),
                new SlamObservation(-10, Vector3.up, Color.white, Quaternion.identity, "123", "f.jpg"),
            };

            sut.OnItemsAdded(null, new AddedEventArgs<SlamObservation>(observations));
            sut.OnItemsUpdated(null, new UpdatedEventArgs<SlamObservation>(moreObservations));
            sut.OnItemsRemoved(null, new RemovedEventArgs<SlamObservation>(new List<int> { 0 }));
            sut.Dispose();

            using var input = File.OpenRead(_filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Add, packet.Action);
            CheckObservations(observations, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            CheckObservations(moreObservations, anotherPacket);

            var yetAnotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Remove, yetAnotherPacket.Action);
            var removingPoints = yetAnotherPacket.ExtractObservations(null, "").ToList();
            Assert.AreEqual(1, removingPoints.Count);
            Assert.AreEqual(0, removingPoints[0].Id);
            CheckMetadata(input, 3);
        }

        [Test]
        public void TrackedObjectsTest()
        {
            var sut = CreateRecorder(2);
            var trackedObjects = new[]
            {
                new SlamTrackedObject(0, Vector3.one, Quaternion.identity, Color.blue),
                new SlamTrackedObject(1, Vector3.down, new Quaternion(1, 2, 3, 1), Color.red),
                new SlamTrackedObject(-10, Vector3.forward, new Quaternion(3, 2, 1, 2), Color.gray, "test message")
            };
            var moreTrackedObjects = new[]
            {
                new SlamTrackedObject(0, Vector3.zero, new Quaternion(5, 6, 7, 0), Color.red, "Another message"),
                new SlamTrackedObject(-10, Vector3.up, Quaternion.identity, Color.white, "test message")
            };

            sut.OnItemsAdded(null, new AddedEventArgs<SlamTrackedObject>(trackedObjects));
            sut.OnItemsUpdated(null, new UpdatedEventArgs<SlamTrackedObject>(moreTrackedObjects));
            sut.OnItemsRemoved(null, new RemovedEventArgs<SlamTrackedObject>(new List<int> { 0 }));
            sut.Dispose();

            using var input = File.OpenRead(_filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Add, packet.Action);
            CheckTrackedObjects(trackedObjects, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            CheckTrackedObjects(moreTrackedObjects, anotherPacket);

            var yetAnotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Remove, yetAnotherPacket.Action);
            var removingPoints = yetAnotherPacket.ExtractTrackedObjects(_converter).ToList();
            Assert.AreEqual(1, removingPoints.Count);
            Assert.AreEqual(0, removingPoints[0].Id);
            CheckMetadata(input, 3);
        }

        [Test]
        public void PlanesTest()
        {
            var sut = CreateRecorder(3);
            var planes = new[]
            {
                new SlamPlane(0, Vector3.one, Vector3.zero, Color.blue),
                new SlamPlane(1, Vector3.down, Vector3.up, Color.red),
                new SlamPlane(-10, Vector3.forward, Vector3.back, Color.gray, "test message")
            };
            var morePlanes = new[]
            {
                new SlamPlane(0, Vector3.zero, Vector3.left, Color.red, "Another message"),
                new SlamPlane(-10, Vector3.up, Vector3.right, Color.white, "test message")
            };

            sut.OnItemsAdded(null, new AddedEventArgs<SlamPlane>(planes));
            sut.OnItemsUpdated(null, new UpdatedEventArgs<SlamPlane>(morePlanes));
            sut.OnItemsRemoved(null, new RemovedEventArgs<SlamPlane>(new List<int> { 0 }));
            sut.Dispose();

            using var input = File.OpenRead(_filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Add, packet.Action);
            CheckPlanes(planes, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            CheckPlanes(morePlanes, anotherPacket);

            var yetAnotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Remove, yetAnotherPacket.Action);
            var removingPoints = yetAnotherPacket.ExtractPlanes(_converter).ToList();
            Assert.AreEqual(1, removingPoints.Count);
            Assert.AreEqual(0, removingPoints[0].Id);
            CheckMetadata(input, 3);
        }

        [Test]
        public void LinesTest()
        {
            var sut = CreateRecorder(4);
            var lines = new[]
            {
                new SlamLine(0, 1),
                new SlamLine(1, 2),
                new SlamLine(-10, -6)
            };
            var moreLines = new[]
            {
                new SlamLine(0, 3),
                new SlamLine(-10, 4)
            };


            sut.OnItemsAdded(null, new AddedEventArgs<SlamLine>(lines));
            sut.OnItemsUpdated(null, new UpdatedEventArgs<SlamLine>(moreLines));
            sut.OnItemsRemoved(null, new RemovedEventArgs<SlamLine>(new List<int> { 0 }));
            sut.Dispose();

            using var input = File.OpenRead(_filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Add, packet.Action);
            CheckLines(lines, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            CheckLines(moreLines, anotherPacket);

            var yetAnotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Remove, yetAnotherPacket.Action);
            var removingPoints = yetAnotherPacket.ExtractLines(_converter).ToList();
            Assert.AreEqual(1, removingPoints.Count);
            Assert.AreEqual(0, removingPoints[0].Id);
            CheckMetadata(input, 3);
        }

        [Test]
        public void StartStopTest()
        {
            var sut = CreateRecorder(5);
            var points = new[]
            {
                new SlamPoint(0, Vector3.one, Color.blue),
                new SlamPoint(1, Vector3.down, Color.red),
                new SlamPoint(-10, Vector3.forward, Color.gray, "test message")
            };
            var morePoints = new[]
            {
                new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                new SlamPoint(-10, Vector3.up, Color.white, "test message")
            };

            sut.OnItemsAdded(null, new AddedEventArgs<SlamPoint>(points));
            sut.OnItemsUpdated(null, new UpdatedEventArgs<SlamPoint>(morePoints));
            sut.Dispose();

            sut.OnItemsRemoved(null, new RemovedEventArgs<SlamPoint>(new List<int> { 0 }));

            using var input = File.OpenRead(_filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Add, packet.Action);
            CheckPoints(points, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            CheckPoints(morePoints, anotherPacket);

            Action act = () => PacketPb.Parser.ParseDelimitedFrom(input);
            act.Should().Throw<InvalidProtocolBufferException>();
            CheckMetadata(input, 2);
        }

        #region Not tests

        // Color have 32 bit size, so when it converts form int to float it has 1/256 ~ 0.004 accuracy.
        private void CheckColors(Color first, Color second)
        {
            float epsilon = 0.004f;
            first.r.Should().BeApproximately(second.r, epsilon);
            first.g.Should().BeApproximately(second.g, epsilon);
            first.b.Should().BeApproximately(second.b, epsilon);
        }

        private void CheckDiffAndPoint(SlamPointDiff diff, SlamPoint point)
        {
            diff.Id.Should().Be(point.Id);
            diff.Position?.Should().Be(point.Position);
            if (diff.Color.HasValue) CheckColors(diff.Color.Value, point.Color);
            if (!string.IsNullOrEmpty(diff.Message)) diff.Message.Should().Be(point.Message);
        }

        private void CheckPoints(IEnumerable<SlamPoint> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Points, packet.DataCase);
            foreach (var (diff, expected) in packet.ExtractPoints(_converter)
                    .Zip(points, (point, item) => (point, item)))
            {
                CheckDiffAndPoint(diff, expected);
            }
        }

        private void CheckDiffAndObservation(SlamObservationDiff diff, SlamObservation observation)
        {
            diff.Id.Should().Be(observation.Id);
            diff.Position?.Should().Be(observation.Position);
            if (diff.Color.HasValue) CheckColors(diff.Color.Value, observation.Color);
            diff.Rotation?.Should().Be(observation.Rotation);
            if (!string.IsNullOrEmpty(diff.Message)) diff.Message.Should().Be(observation.Message);
            if (!string.IsNullOrEmpty(diff.FileName)) diff.FileName.Should().Be(observation.FileName);
        }

        private void CheckObservations(IEnumerable<SlamObservation> observations, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Observations, packet.DataCase);
            foreach (var (diff, expected) in packet.ExtractObservations(_converter, "")
                    .Zip(observations, (point, item) => (point, item)))
            {
                CheckDiffAndObservation(diff, expected);
            }
        }

        private void CheckDiffAndTrackedObj(SlamTrackedObjectDiff diff, SlamTrackedObject obj)
        {
            diff.Id.Should().Be(obj.Id);
            diff.Position?.Should().Be(obj.Position);
            diff.Rotation?.Should().Be(obj.Rotation);
            if (diff.Color.HasValue) CheckColors(diff.Color.Value, obj.Color);
            if (!string.IsNullOrEmpty(diff.Message)) diff.Message.Should().Be(obj.Message);
        }

        private void CheckTrackedObjects(IEnumerable<SlamTrackedObject> objs, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.TrackedObjs, packet.DataCase);
            foreach (var (diff, expected) in packet.ExtractTrackedObjects(_converter)
                    .Zip(objs, (point, item) => (point, item)))
            {
                CheckDiffAndTrackedObj(diff, expected);
            }
        }

        private void CheckDiffAndPlane(SlamPlaneDiff diff, SlamPlane plane)
        {
            diff.Id.Should().Be(plane.Id);
            diff.Offset?.Should().Be(plane.Offset);
            diff.Normal?.Should().Be(plane.Normal);
            if (diff.Color.HasValue) CheckColors(diff.Color.Value, plane.Color);
            if (!string.IsNullOrEmpty(diff.Message)) diff.Message.Should().Be(plane.Message);
        }

        private void CheckPlanes(IEnumerable<SlamPlane> planes, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Planes, packet.DataCase);
            foreach (var (diff, expected) in packet.ExtractPlanes(_converter)
                    .Zip(planes, (point, item) => (point, item)))
            {
                CheckDiffAndPlane(diff, expected);
            }
        }

        private void CheckDiffAndLine(SlamLineDiff diff, SlamLine line)
        {
            CheckDiffAndPoint(diff.Point1, line.Point1);
            CheckDiffAndPoint(diff.Point2, line.Point2);
        }

        private void CheckLines(IEnumerable<SlamLine> lines, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Lines, packet.DataCase);
            foreach (var (diff, expected) in packet.ExtractLines(_converter)
                    .Zip(lines, (point, item) => (point, item)))
            {
                CheckDiffAndLine(diff, expected);
            }
        }

        private void CheckMetadata(FileStream input, int expectedFrames)
        {
            input.Position = input.Length - 8;
            var buffer = new byte[4];
            input.Read(buffer, 0, 4);
            buffer.Should().BeEquivalentTo(BitConverter.GetBytes(ProtobufRecorder.Marker));
            input.Read(buffer, 0, 4);
            buffer.Should().BeEquivalentTo(BitConverter.GetBytes(expectedFrames));
        }

        private ProtobufRecorder CreateRecorder(int id)
        {
            _filename = $"test_{id}.dat";
            var factory = new ProtobufRecorderFactory { Filename = _filename };
            return (ProtobufRecorder)factory.Start();
        }

        #endregion
    }
}