﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Recorder;
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
            ICloudItem[] points =
            {
                new SlamPoint(0, Vector3.one, Color.blue),
                new SlamPoint(1, Vector3.down, Color.red),
                new SlamPoint(-10, Vector3.forward, Color.gray, "test message")
            };
            ICloudItem[] morePoints =
            {
                new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                new SlamPoint(-10, Vector3.up, Color.white, "test message")
            };

            Record<SlamPoint>(points, morePoints);

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
            ICloudItem[] observations =
            {
                new SlamObservation(new SlamPoint(0, Vector3.one, Color.blue), Quaternion.identity, "123", "f.jpg"),
                new SlamObservation(new SlamPoint(1, Vector3.down, Color.red), new Quaternion(0, 5, 15, 3), "321", ""),
                new SlamObservation(new SlamPoint(-10, Vector3.forward, Color.gray, "test message"),
                                    new Quaternion(59, 46, 3, 24), "adsf", "f.jpg"),
            };
            ICloudItem[] moreObservations =
            {
                new SlamObservation(new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                                    new Quaternion(6, 2, 1, 0), "123", "f.jpg"),
                new SlamObservation(new SlamPoint(-10, Vector3.up, Color.white, "test message"), Quaternion.identity,
                                    "123", "f.jpg"),
            };

            Record<SlamObservation>(observations, moreObservations);

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
            ICloudItem[] trackedObjects =
            {
                new SlamTrackedObject(0, Vector3.one, Quaternion.identity, Color.blue),
                new SlamTrackedObject(1, Vector3.down, new Quaternion(1, 2, 3, 1), Color.red),
                new SlamTrackedObject(-10, Vector3.forward, new Quaternion(3, 2, 1, 2), Color.gray, "test message")
            };
            ICloudItem[] moreTrackedObjects =
            {
                new SlamTrackedObject(0, Vector3.zero, new Quaternion(5, 6, 7, 0), Color.red, "Another message"),
                new SlamTrackedObject(-10, Vector3.up, Quaternion.identity, Color.white, "test message")
            };

            Record<SlamTrackedObject>(trackedObjects, moreTrackedObjects);

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
            ICloudItem[] planes =
            {
                new SlamInfinitePlane(0, Vector3.one, Vector3.zero, Color.blue),
                new SlamInfinitePlane(1, Vector3.down, Vector3.up, Color.red),
                new SlamInfinitePlane(-10, Vector3.forward, Vector3.back, Color.gray, "test message")
            };
            ICloudItem[] morePlanes =
            {
                new SlamInfinitePlane(0, Vector3.zero, Vector3.left, Color.red, "Another message"),
                new SlamInfinitePlane(-10, Vector3.up, Vector3.right, Color.white, "test message")
            };

            Record<SlamInfinitePlane>(planes, morePlanes);

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
            ICloudItem[] lines =
            {
                new SlamLine(0, 1),
                new SlamLine(1, 2),
                new SlamLine(-10, -6)
            };
            ICloudItem[] moreLines =
            {
                new SlamLine(0, 3),
                new SlamLine(-10, 4)
            };

            Record<SlamLine>(lines, moreLines);

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
        public void ConnectionsTest()
        {
            (int id1, int id2)[] connections = {(0, 1), (0, 3), (3, 2), (-10, 9)};
            (int id1, int id2)[] removed = {connections[1], connections[3]};
            IDataRecorderPlugin recorder = new ProtobufRecorder();
            recorder.FileName = Filename;
            recorder.StartRecording();
            recorder.OnConnectionsUpdated("Connections", connections);
            recorder.OnConnectionsRemoved("Connections", removed);
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
            ICloudItem[] points =
            {
                new SlamPoint(0, Vector3.one, Color.blue),
                new SlamPoint(1, Vector3.down, Color.red),
                new SlamPoint(-10, Vector3.forward, Color.gray, "test message")
            };
            (int id1, int id2)[] connections = {(0, 1), (0, 3), (3, 2), (-10, 9)};
            
            File.Delete(Filename);
            IDataRecorderPlugin recorder = new ProtobufRecorder();
            recorder.FileName = Filename;
            recorder.OnAdded("", points);
            recorder.OnUpdated("", points);
            recorder.OnRemoved("", typeof(SlamPoint), points.Select(p => p.Id).ToList());
            recorder.OnConnectionsUpdated("", connections);
            recorder.OnConnectionsRemoved("", connections);
            Assert.IsFalse(File.Exists(Filename));
            recorder.StartRecording();
            recorder.OnAdded("", points);
            recorder.OnUpdated("", points);
            recorder.OnRemoved("", typeof(SlamPoint), points.Select(p => p.Id).ToList());
            recorder.OnConnectionsUpdated("", connections);
            recorder.OnConnectionsRemoved("", connections);
            Assert.IsTrue(File.Exists(Filename));;
            recorder.StopRecording();
            using var input = File.OpenRead(Filename);
            CheckMetadata(input, 5);
        }

        [Test]
        public void FailedTest()
        {
            Assert.Fail();
        }

        #region Not tests

        private void CheckPoints(IEnumerable<ICloudItem> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Points, packet.DataCase);
            Assert.IsTrue(packet.ExtractPoints()
                                  .Zip(points, (point, item) => (point, (SlamPoint) item))
                                  .All(d => d.point.Equals(d.Item2)));
        }

        private void CheckObservations(IEnumerable<ICloudItem> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Observations, packet.DataCase);
            Assert.IsTrue(packet.ExtractObservations(null, "")
                                  .Zip(points, (point, item) => (point, (SlamObservation) item))
                                  .All(d => d.point.Equals(d.Item2)));
        }

        private void CheckTrackedObjects(IEnumerable<ICloudItem> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.TrackedObjs, packet.DataCase);
            Assert.IsTrue(packet.ExtractTrackedObjects()
                                  .Zip(points, (point, item) => (point, (SlamTrackedObject) item))
                                  .All(d => d.point.Equals(d.Item2)));
        }

        private void CheckPlanes(IEnumerable<ICloudItem> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.InfinitePlanes, packet.DataCase);
            Assert.IsTrue(packet.ExtractInfinitePlanes()
                                  .Zip(points, (point, item) => (point, (SlamInfinitePlane) item))
                                  .All(d => d.point.Equals(d.Item2)));
        }

        private void CheckLines(IEnumerable<ICloudItem> points, PacketPb packet)
        {
            Assert.AreEqual(PacketPb.DataOneofCase.Lines, packet.DataCase);
            Assert.IsTrue(packet.ExtractLines()
                                  .Zip(points, (point, item) => (point, (SlamLine) item))
                                  .All(d => d.point.Equals(d.Item2)));
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

        private void Record<T>(ICloudItem[] first, ICloudItem[] second) where T : ICloudItem
        {
            IDataRecorderPlugin recorder = new ProtobufRecorder();
            recorder.FileName = Filename;
            recorder.StartRecording();
            recorder.OnAdded(typeof(T).Name, first);
            recorder.OnUpdated(typeof(T).Name, second);
            recorder.OnRemoved(typeof(T).Name, typeof(T), new List<int> {0});
            recorder.StopRecording();
            Assert.IsTrue(File.Exists(Filename));
        }

        #endregion
    }
}