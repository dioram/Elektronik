using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Elektronik.ProtobufPlugin.Common;
using Google.Protobuf;

namespace Protobuf.Tests
{
    public class PointsStressTests : TestsBase
    {
        private PointPb[] m_map;
        private ConnectionPb[] m_connections;
        private string filename = $"{nameof(PointsStressTests)}.dat";

        public PointsStressTests()
        {
            var rand = new Random();
            m_map = Enumerable.Range(0, 20000).Select(id => new PointPb()
            {
                Id = id,
                Message = $"{id}",
                Position = new Vector3Pb() { X = rand.NextDouble(), Y = rand.NextDouble(), Z = rand.NextDouble(), },
                Color = new ColorPb {B = rand.Next(255), G = rand.Next(255), R = rand.Next(255)},
            }).ToArray();

            m_connections = Enumerable.Range(0, 5000).Select(_ => new ConnectionPb() { Id1 = rand.Next(0, 19999), Id2 = rand.Next(0, 19999), }).ToArray();
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.Add(m_map);

            var t = new Stopwatch();
            t.Start();
            var response = MapClient.Handle(packet);
            t.Stop();
            TestContext.WriteLine($"Handle packet: {t.ElapsedMilliseconds} ms");

            using var file = File.Open(filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };

            packet.Points.Data.Add(m_map);

            var t = new Stopwatch();
            t.Start();
            var response = MapClient.Handle(packet);
            t.Stop();
            TestContext.WriteLine($"Handle packet: {t.ElapsedMilliseconds} ms");

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3)]
        public void UpdateConnections()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            packet.Connections.Data.Add(m_connections);

            var t = new Stopwatch();
            t.Start();
            var response = MapClient.Handle(packet);
            t.Stop();
            TestContext.WriteLine($"Handle packet: {t.ElapsedMilliseconds} ms");

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Remove,
                Points = new PacketPb.Types.Points(),
            };

            packet.Points.Data.Add(m_map.Take(5000));

            var t = new Stopwatch();
            t.Start();
            var response = MapClient.Handle(packet);
            t.Stop();
            TestContext.WriteLine($"Handle packet: {t.ElapsedMilliseconds} ms");

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(5)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                Points = new PacketPb.Types.Points(),
            };

            var t = new Stopwatch();
            t.Start();
            var response = MapClient.Handle(packet);
            t.Stop();
            TestContext.WriteLine($"Handle packet: {t.ElapsedMilliseconds} ms");

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}
