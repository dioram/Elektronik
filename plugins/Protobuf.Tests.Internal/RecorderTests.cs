using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Recorders;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class RecorderTests
    {
        private const string Filename = @"./test.dat";

        [TearDown]
        public void TearDown()
        {
            File.Delete(Filename);
        }

        [Test]
        public void PointsTest()
        {
            var points = new []
            {
                new SlamPoint(0, Vector3.one, Color.blue),
                new SlamPoint(1, Vector3.down, Color.red),
                new SlamPoint(-10, Vector3.forward, Color.gray, "test message")
            };
            var morePoints = new []
            {
                new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                new SlamPoint(-10, Vector3.up, Color.white, "test message")
            };

            Record(points, morePoints);

            using var input = File.OpenRead(Filename);
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
            var observations = new []
            {
                new SlamObservation(new SlamPoint(0, Vector3.one, Color.blue), Quaternion.identity, "123", "f.jpg"),
                new SlamObservation(new SlamPoint(1, Vector3.down, Color.red), new Quaternion(0, 5, 15, 3), "321", ""),
                new SlamObservation(new SlamPoint(-10, Vector3.forward, Color.gray, "test message"),
                                    new Quaternion(59, 46, 3, 24), "adsf", "f.jpg"),
            };
            var moreObservations = new []
            {
                new SlamObservation(new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                                    new Quaternion(6, 2, 1, 0), "123", "f.jpg"),
                new SlamObservation(new SlamPoint(-10, Vector3.up, Color.white, "test message"), Quaternion.identity,
                                    "123", "f.jpg"),
            };

            Record(observations, moreObservations);

            using var input = File.OpenRead(Filename);
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
            var trackedObjects = new []
            {
                new SlamTrackedObject(0, Vector3.one, Quaternion.identity, Color.blue),
                new SlamTrackedObject(1, Vector3.down, new Quaternion(1, 2, 3, 1), Color.red),
                new SlamTrackedObject(-10, Vector3.forward, new Quaternion(3, 2, 1, 2), Color.gray, "test message")
            };
            var moreTrackedObjects = new []
            {
                new SlamTrackedObject(0, Vector3.zero, new Quaternion(5, 6, 7, 0), Color.red, "Another message"),
                new SlamTrackedObject(-10, Vector3.up, Quaternion.identity, Color.white, "test message")
            };

            Record(trackedObjects, moreTrackedObjects);

            using var input = File.OpenRead(Filename);
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
        public void InfinitePlanesTest()
        {
            var planes = new []
            {
                new SlamInfinitePlane(0, Vector3.one, Vector3.zero, Color.blue),
                new SlamInfinitePlane(1, Vector3.down, Vector3.up, Color.red),
                new SlamInfinitePlane(-10, Vector3.forward, Vector3.back, Color.gray, "test message")
            };
            var morePlanes = new []
            {
                new SlamInfinitePlane(0, Vector3.zero, Vector3.left, Color.red, "Another message"),
                new SlamInfinitePlane(-10, Vector3.up, Vector3.right, Color.white, "test message")
            };

            Record(planes, morePlanes);

            using var input = File.OpenRead(Filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Add, packet.Action);
            CheckPlanes(planes, packet);

            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            CheckPlanes(morePlanes, anotherPacket);

            var yetAnotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Remove, yetAnotherPacket.Action);
            var removingPoints = yetAnotherPacket.ExtractInfinitePlanes().ToList();
            Assert.AreEqual(1, removingPoints.Count);
            Assert.AreEqual(0, removingPoints[0].Id);
            CheckMetadata(input, 3);
        }

        [Test]
        public void LinesTest()
        {
            var lines = new []
            {
                new SlamLine(0, 1),
                new SlamLine(1, 2),
                new SlamLine(-10, -6)
            };
            var moreLines = new []
            {
                new SlamLine(0, 3),
                new SlamLine(-10, 4)
            };

            Record(lines, moreLines);

            using var input = File.OpenRead(Filename);
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
        public void PointConnectionsTest()
        {
            (int id1, int id2)[] connections = {(0, 1), (0, 3), (3, 2), (-10, 9)};
            (int id1, int id2)[] removed = {connections[1], connections[3]};
            IDataRecorderPlugin recorder = new ProtobufRecorder("", null, new FakeConverter());
            recorder.FileName = Filename;
            recorder.StartRecording();
            recorder.OnConnectionsUpdated<SlamPoint>("Connections", connections);
            recorder.OnConnectionsRemoved<SlamPoint>("Connections", removed);
            recorder.StopRecording();
            Assert.IsTrue(File.Exists(Filename));

            using var input = File.OpenRead(Filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, packet.Action);
            Assert.AreEqual(PacketPb.Types.Connections.Types.Action.Add, packet.Connections.Action);
            CheckConnections(connections, packet);
            
            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            Assert.AreEqual(PacketPb.Types.Connections.Types.Action.Remove, anotherPacket.Connections.Action);
            CheckConnections(removed, anotherPacket);
            CheckMetadata(input, 2);
        }

        [Test]
        public void ObservationsConnectionsTest()
        {
            (int id1, int id2)[] connections = {(0, 1), (0, 3), (3, 2), (-10, 9)};
            (int id1, int id2)[] removed = {connections[1], connections[3]};
            IDataRecorderPlugin recorder = new ProtobufRecorder("", null, new FakeConverter());
            recorder.FileName = Filename;
            recorder.StartRecording();
            recorder.OnConnectionsUpdated<SlamObservation>("Connections", connections);
            recorder.OnConnectionsRemoved<SlamObservation>("Connections", removed);
            recorder.StopRecording();
            Assert.IsTrue(File.Exists(Filename));

            using var input = File.OpenRead(Filename);
            var packet = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, packet.Action);
            Assert.AreEqual(PacketPb.Types.Connections.Types.Action.Add, packet.Connections.Action);
            CheckConnections(connections, packet);
            
            var anotherPacket = PacketPb.Parser.ParseDelimitedFrom(input);
            Assert.AreEqual(PacketPb.Types.ActionType.Update, anotherPacket.Action);
            Assert.AreEqual(PacketPb.Types.Connections.Types.Action.Remove, anotherPacket.Connections.Action);
            CheckConnections(removed, anotherPacket);
            CheckMetadata(input, 2);
        }

        [Test]
        public void StartStopTest()
        {
            SlamPoint[] points =
            {
                new SlamPoint(0, Vector3.one, Color.blue),
                new SlamPoint(1, Vector3.down, Color.red),
                new SlamPoint(-10, Vector3.forward, Color.gray, "test message")
            };
            (int id1, int id2)[] connections = {(0, 1), (0, 3), (3, 2), (-10, 9)};
            
            File.Delete(Filename);
            IDataRecorderPlugin recorder = new ProtobufRecorder("", null, new FakeConverter());
            recorder.FileName = Filename;
            recorder.OnAdded("", points);
            recorder.OnUpdated("", points);
            recorder.OnRemoved<SlamPoint>("", points.Select(p => p.Id).ToList());
            recorder.OnConnectionsUpdated<SlamPoint>("", connections);
            recorder.OnConnectionsRemoved<SlamPoint>("", connections);
            Assert.IsFalse(File.Exists(Filename));
            recorder.StartRecording();
            recorder.OnAdded("", points);
            recorder.OnUpdated("", points);
            recorder.OnRemoved<SlamPoint>("", points.Select(p => p.Id).ToList());
            recorder.OnConnectionsUpdated<SlamPoint>("", connections);
            recorder.OnConnectionsRemoved<SlamPoint>("", connections);
            Assert.IsTrue(File.Exists(Filename));
            recorder.StopRecording();
            using var input = File.OpenRead(Filename);
            CheckMetadata(input, 5);
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
        
        private bool CheckDiffAndPlane(SlamInfinitePlaneDiff diff, SlamInfinitePlane plane)
        {
            bool id = diff.Id == plane.Id;
            bool offset = !diff.Offset.HasValue || diff.Offset.Value == plane.Offset;
            bool normal = !diff.Normal.HasValue || diff.Normal.Value == plane.Normal;
            bool color = !diff.Color.HasValue || AreColorsEqual(diff.Color.Value, plane.Color);
            bool message = string.IsNullOrEmpty(diff.Message) || diff.Message == plane.Message;
            return id && offset && normal && color && message;
        }
        
        private void CheckPlanes(IEnumerable<SlamInfinitePlane> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.InfinitePlanes, packet.DataCase);
            Assert.IsTrue(packet.ExtractInfinitePlanes()
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
        
        private void CheckConnections(IEnumerable<(int id1, int id2)> connections, PacketPb packet)
        {
            Assert.IsTrue(packet.Connections.Data
                                  .Zip(connections, (pb, item) => ((pb.Id1, pb.Id2), item))
                                  .All(d => d.Item1.Equals(d.Item2)));
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

        private void Record<TCloudItem>(TCloudItem[] first, TCloudItem[] second) where TCloudItem : struct, ICloudItem
        {
            IDataRecorderPlugin recorder = new ProtobufRecorder("", null, new FakeConverter());
            recorder.FileName = Filename;
            recorder.StartRecording();
            recorder.OnAdded(typeof(TCloudItem).Name, first);
            recorder.OnUpdated(typeof(TCloudItem).Name, second);
            recorder.OnRemoved<TCloudItem>(typeof(TCloudItem).Name, new List<int> {0});
            recorder.StopRecording();
            Assert.IsTrue(File.Exists(Filename));
        }

        #endregion
    }
}