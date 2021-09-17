using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
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
            Assert.AreEqual(PacketPb.Types.ActionType.Add, packet.Action);
            CheckPoints(points, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            CheckPoints(morePoints, anotherPacket);

            var yetAnotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Remove, yetAnotherPacket.Action);
            var removingPoints = yetAnotherPacket.ExtractPoints().ToList();
            Assert.AreEqual(1, removingPoints.Count);
            Assert.AreEqual(0, removingPoints[0].Id);
            CheckMetadata(input, 3);
        }

        [Test]
        public void ObservationsTest()
        {
            var sut = CreateRecorder(1);
            var observations = new[]
            {
                new SlamObservation(new SlamPoint(0, Vector3.one, Color.blue), Quaternion.identity, "123", "f.jpg"),
                new SlamObservation(new SlamPoint(1, Vector3.down, Color.red), new Quaternion(0, 5, 15, 3), "321", ""),
                new SlamObservation(new SlamPoint(-10, Vector3.forward, Color.gray, "test message"),
                                    new Quaternion(59, 46, 3, 24), "adsf", "f.jpg"),
            };
            var moreObservations = new[]
            {
                new SlamObservation(new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                                    new Quaternion(6, 2, 1, 0), "123", "f.jpg"),
                new SlamObservation(new SlamPoint(-10, Vector3.up, Color.white, "test message"), Quaternion.identity,
                                    "123", "f.jpg"),
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
            var removingPoints = yetAnotherPacket.ExtractTrackedObjects().ToList();
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
            var removingPoints = yetAnotherPacket.ExtractPlanes().ToList();
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
            var removingPoints = yetAnotherPacket.ExtractLines().ToList();
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
        private bool AreColorsEqual(Color first, Color second)
        {
            float epsilon = 0.004f;
            return (Mathf.Abs(first.r - second.r) < epsilon)
                    && (Mathf.Abs(first.g - second.g) < epsilon)
                    && (Mathf.Abs(first.b - second.b) < epsilon);
        }

        private bool CheckDiffAndPoint(SlamPointDiff diff, SlamPoint point)
        {
            bool id = diff.Id == point.Id;
            bool offset = !diff.Position.HasValue || diff.Position.Value == point.Position;
            bool color = !diff.Color.HasValue || AreColorsEqual(diff.Color.Value, point.Color);
            bool message = string.IsNullOrEmpty(diff.Message) || diff.Message == point.Message;
            return id && offset && color && message;
        }

        private void CheckPoints(IEnumerable<SlamPoint> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Points, packet.DataCase);
            Assert.IsTrue(packet.ExtractPoints()
                                  .Zip(points, (point, item) => (point, item))
                                  .All(d => CheckDiffAndPoint(d.point, d.item)));
        }

        private bool CheckDiffAndObservation(SlamObservationDiff diff, SlamObservation observation)
        {
            bool id = diff.Id == observation.Id;
            bool point = CheckDiffAndPoint(diff.Point, observation.Point);
            bool offset = !diff.Rotation.HasValue || diff.Rotation.Value == observation.Rotation;
            bool message = string.IsNullOrEmpty(diff.Message) || diff.Message == observation.Message;
            bool filename = string.IsNullOrEmpty(diff.FileName) || diff.FileName == observation.FileName;
            return id && offset && point && message && filename;
        }

        private void CheckObservations(IEnumerable<SlamObservation> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Observations, packet.DataCase);
            Assert.IsTrue(packet.ExtractObservations(null, "")
                                  .Zip(points, (point, item) => (point, item))
                                  .All(d => CheckDiffAndObservation(d.point, d.item)));
        }

        private bool CheckDiffAndTrackedObj(SlamTrackedObjectDiff diff, SlamTrackedObject obj)
        {
            bool id = diff.Id == obj.Id;
            bool offset = !diff.Position.HasValue || diff.Position.Value == obj.Position;
            bool normal = !diff.Rotation.HasValue || diff.Rotation.Value == obj.Rotation;
            bool color = !diff.Color.HasValue || AreColorsEqual(diff.Color.Value, obj.Color);
            bool message = string.IsNullOrEmpty(diff.Message) || diff.Message == obj.Message;
            return id && offset && normal && color && message;
        }

        private void CheckTrackedObjects(IEnumerable<SlamTrackedObject> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.TrackedObjs, packet.DataCase);
            Assert.IsTrue(packet.ExtractTrackedObjects()
                                  .Zip(points, (point, item) => (point, item))
                                  .All(d => CheckDiffAndTrackedObj(d.point, d.item)));
        }

        private bool CheckDiffAndPlane(SlamPlaneDiff diff, SlamPlane plane)
        {
            bool id = diff.Id == plane.Id;
            bool offset = !diff.Offset.HasValue || diff.Offset.Value == plane.Offset;
            bool normal = !diff.Normal.HasValue || diff.Normal.Value == plane.Normal;
            bool color = !diff.Color.HasValue || AreColorsEqual(diff.Color.Value, plane.Color);
            bool message = string.IsNullOrEmpty(diff.Message) || diff.Message == plane.Message;
            return id && offset && normal && color && message;
        }

        private void CheckPlanes(IEnumerable<SlamPlane> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Planes, packet.DataCase);
            Assert.IsTrue(packet.ExtractPlanes()
                                  .Zip(points, (point, item) => (point, item))
                                  .All(pair => CheckDiffAndPlane(pair.point, pair.item)));
        }

        private bool CheckDiffAndLine(SlamLineDiff diff, SlamLine line)
        {
            bool point1 = CheckDiffAndPoint(diff.Point1, line.Point1);
            bool point2 = CheckDiffAndPoint(diff.Point2, line.Point2);
            return point1 && point2;
        }

        private void CheckLines(IEnumerable<SlamLine> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Lines, packet.DataCase);
            Assert.IsTrue(packet.ExtractLines()
                                  .Zip(points, (point, item) => (point, item))
                                  .All(d => CheckDiffAndLine(d.point, d.item)));
        }

        private void CheckMetadata(FileStream input, int expectedFrames)
        {
            input.Position = input.Length - 8;
            var buffer = new byte[4];
            input.Read(buffer, 0, 4);
            Assert.AreEqual(BitConverter.GetBytes(ProtobufRecorder.Marker), buffer);
            input.Read(buffer, 0, 4);
            Assert.AreEqual(BitConverter.GetBytes(expectedFrames), buffer);
        }

        private ProtobufRecorder CreateRecorder(int id)
        {
            _filename = $"test_{id}.dat";
            var factory = new ProtobufRecorderFactory() { Filename = _filename };
            return (ProtobufRecorder)factory.Start(new FakeConverter());
        }

        #endregion
    }
}