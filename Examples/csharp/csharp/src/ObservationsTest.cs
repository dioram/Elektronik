using Elektronik.Common.Data.Pb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csharp
{
    public class ObservationsTest : ITest
    {
        // imitate slam maps
        private ObservationPb[] m_observationMap;
        private ConnectionPb[] m_connectionMap;

        public ObservationsTest()
        {
            m_observationMap = Enumerable.Range(0, 5).Select(id => new ObservationPb()
            {
                Point = new PointPb()
                {
                    Id = id,
                    Message = $"{id}",
                },
                Orientation = new Vector4Pb() { X = 1, }
            }).ToArray();

            m_observationMap[0].Point.Position = new Vector3Pb() { X = 0, Y = .5 };
            m_observationMap[1].Point.Position = new Vector3Pb() { X = .5, Y = -.5 };
            m_observationMap[2].Point.Position = new Vector3Pb() { X = -.5, Y = -.5 };
            m_observationMap[3].Point.Position = new Vector3Pb() { X = -.5, Y = 0 };
            m_observationMap[4].Point.Position = new Vector3Pb() { X = .5, Y = 0 };

            m_connectionMap = new[]
            {
                new ConnectionPb() { Pt1 = m_observationMap[0], Pt2 = m_observationMap[1], },
                new ConnectionPb() { Pt1 = m_observationMap[0], Pt2 = m_observationMap[2], },
                new ConnectionPb() { Pt1 = m_observationMap[2], Pt2 = m_observationMap[4], },
                new ConnectionPb() { Pt1 = m_observationMap[1], Pt2 = m_observationMap[3], },
                new ConnectionPb() { Pt1 = m_observationMap[3], Pt2 = m_observationMap[4], },
            };
        }

        public IEnumerable<PacketPb> Create()
        {
            var pointsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                ObservationsPacket = new PacketPb.Types.ObservationsPacket(),
            };
            pointsPacket.ObservationsPacket.Observations.Add(m_observationMap);

            var connectionsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket()
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Observations,
                },
            };


            connectionsPacket.ConnectionsPacket.Connections.Add(m_connectionMap);

            

            return new[] { pointsPacket, connectionsPacket };
        }

        public IEnumerable<PacketPb> Update()
        {
            m_observationMap[0].Point.Position.Z += .5;
            m_observationMap[2].Point.Position.Z += .5;
            m_observationMap[4].Point.Position.Z += .5;

            var pointsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                ObservationsPacket = new PacketPb.Types.ObservationsPacket(),
            };
            pointsPacket.ObservationsPacket.Observations.Add(m_observationMap);
            
            var connectionsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket()
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Observations,
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
                ObservationsPacket = new PacketPb.Types.ObservationsPacket(),
            };

            pointsPacket.ObservationsPacket.Observations.Add(new[] { m_observationMap[1], m_observationMap[3] });

            var connectionsPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket()
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Observations,
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
                ObservationsPacket = new PacketPb.Types.ObservationsPacket(),
            };

            var clearLinesPacket = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                ConnectionsPacket = new PacketPb.Types.ConnectionsPacket() 
                {
                    Map = PacketPb.Types.ConnectionsPacket.Types.MapType.Observations,
                },
            };

            return new[] { clearPointsPacket, clearLinesPacket };
        }
    }
}
