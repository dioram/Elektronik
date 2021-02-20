using System.IO;
using System.Linq;
using Elektronik.Common.Data.Pb;
using Google.Protobuf;
using NUnit.Framework;

namespace csharp.Tests
{
    public class InfoTests : TestsBase
    {
        private string filename = $"{nameof(InfoTests)}.dat";
        private PointPb[] m_map;
        
        public InfoTests()
        {
            m_map = Enumerable.Range(0, 5).Select(id => new PointPb()
            {
                    Id = id,
                    Message = $"{id}",
            }).ToArray();

            m_map[0].Position = new Vector3Pb() { X = 0, Y = .5 };
            m_map[1].Position = new Vector3Pb() { X = .5, Y = -.5 };
            m_map[2].Position = new Vector3Pb() { X = -.5, Y = -.5 };
            m_map[3].Position = new Vector3Pb() { X = -.5, Y = 0 };
            m_map[4].Position = new Vector3Pb() { X = .5, Y = 0 };
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
            packet.Points.Data.Add(m_map);

            using var file = File.Open(filename, FileMode.Create);
            packet.WriteDelimitedTo(file);
        }
        
        [Test, Order(2), Repeat(10)]
        public void SendInfoPackage()
        {
            
            var packet = new PacketPb()
            {
                    Action = PacketPb.Types.ActionType.Info,
                    Points = new PacketPb.Types.Points(),
                    Message = "Info package",
                    Special = true,
            };
            packet.Points.Data.Add(m_map);

            using var file = File.Open(filename, FileMode.Append);
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
            m_map = m_map.Select(p =>
            {
                p.Id = m_map.Length + p.Id;
                return p;
            }).ToArray();
            packet.Points.Data.Add(m_map);

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
        }
    }
}