using System.IO;
using System.Linq;
using Elektronik.Common.Data.Pb;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests
{
    public class InfoTests : TestsBase
    {
        private readonly string _filename = $"{nameof(InfoTests)}.dat";
        private PointPb[] _map;

        public InfoTests()
        {
            _map = Enumerable.Range(0, 5).Select(id => new PointPb()
            {
                Id = id,
                Message = $"{id}",
            }).ToArray();

            _map[0].Position = new Vector3Pb() {X = 0, Y = .5};
            _map[1].Position = new Vector3Pb() {X = .5, Y = -.5};
            _map[2].Position = new Vector3Pb() {X = -.5, Y = -.5};
            _map[3].Position = new Vector3Pb() {X = -.5, Y = 0};
            _map[4].Position = new Vector3Pb() {X = .5, Y = 0};
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
                Special = true,
            };
            packet.Points.Data.Add(_map);

            using var file = File.Open(_filename, FileMode.Create);
            packet.WriteDelimitedTo(file);
        }

        [Test, Order(2), Repeat(10)]
        public void SendInfoPackage()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Info,
                Points = new PacketPb.Types.Points(),
                Message = "This is package with info message",
                Special = true,
            };
            packet.Points.Data.Add(_map);

            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
        }

        [Test, Order(3)]
        public void Add()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
                Special = true,
            };
            _map = _map.Select(p =>
            {
                p.Id = _map.Length + p.Id;
                return p;
            }).ToArray();
            packet.Points.Data.Add(_map);

            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
        }
    }
}