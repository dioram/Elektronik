using Elektronik.Common.Data.Pb;
using Google.Protobuf.Collections;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace csharp
{
    public class PointsTest : ITest
    {
        // imitate slam maps
        private PointPb[] m_pointMap;
        private ConnectionPb[] m_connectionMap;

        public PointsTest()
        {
            m_pointMap = Enumerable.Range(0, 5).Select(id => new PointPb()
            {
                Id = id,
                Message = $"{id}",
            }).ToArray();

            m_pointMap[0].Position = new Vector3Pb() { X = 0, Y = .5 };
            m_pointMap[1].Position = new Vector3Pb() { X = .5, Y = -.5 };
            m_pointMap[2].Position = new Vector3Pb() { X = -.5, Y = -.5 };
            m_pointMap[3].Position = new Vector3Pb() { X = -.5, Y = 0 };
            m_pointMap[4].Position = new Vector3Pb() { X = .5, Y = 0 };

            m_connectionMap = new[]
            {
                new ConnectionPb() { Pt1 = m_pointMap[0], Pt2 = m_pointMap[1], },
                new ConnectionPb() { Pt1 = m_pointMap[0], Pt2 = m_pointMap[2], },
                new ConnectionPb() { Pt1 = m_pointMap[2], Pt2 = m_pointMap[4], },
                new ConnectionPb() { Pt1 = m_pointMap[1], Pt2 = m_pointMap[3], },
                new ConnectionPb() { Pt1 = m_pointMap[3], Pt2 = m_pointMap[4], },
            };
        }

        public IEnumerable<PacketPb> Create()
        {
            var pointsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                PointsPacket = new PacketPb.Types.PointsPacket(),
            };
            pointsPacket.PointsPacket.Points.Add(m_pointMap);

            var connectionsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket()
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Points,
                },
            };
            connectionsPacket.ConnectionsPacket.Connections.Add(m_connectionMap);

            var ptsInfo = pointsPacket.Clone();
            ptsInfo.Action = PacketPb.Types.ActionType.Info;
            ptsInfo.Message = "Added points";
            ptsInfo.Special = true;

            return new[] { pointsPacket, connectionsPacket, ptsInfo };
        }

        public IEnumerable<PacketPb> Update()
        {
            m_pointMap[0].Position.Z += .5;
            m_pointMap[2].Position.Z += .5;
            m_pointMap[4].Position.Z += .5;

            var pointsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                PointsPacket = new PacketPb.Types.PointsPacket(),
            };
            pointsPacket.PointsPacket.Points.Add(m_pointMap);

            var connectionsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket()
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Points,
                },
            };
            connectionsPacket.ConnectionsPacket.Connections.Add(m_connectionMap);

            return new[] { pointsPacket, connectionsPacket };
        }

        public IEnumerable<PacketPb> Remove()
        {
            var pointsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                PointsPacket = new PacketPb.Types.PointsPacket(),
            };

            pointsPacket.PointsPacket.Points.Add(new[] { m_pointMap[1], m_pointMap[3] });

            var connectionsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket()
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Points,
                },
            };

            connectionsPacket.ConnectionsPacket.Connections.Add(new[] { m_connectionMap[0], m_connectionMap[3], m_connectionMap[4], });
            return new[] { pointsPacket, connectionsPacket };
        }

        public IEnumerable<PacketPb> Clear()
        {
            var clearPointsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                PointsPacket = new PacketPb.Types.PointsPacket(),
            };
            var clearLinesPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket() 
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Points 
                },
            };

            return new[] { clearPointsPacket, clearLinesPacket };
        }
    }
}
