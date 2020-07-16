using Elektronik.Offline.Loggers;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Presenters;
using Elektronik.Common.Data;
using Elektronik.Common.Maps;
using System.Collections.Generic;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using System.Linq;
using Elektronik.Common.Data.Pb;
using System;
using TMPro;

namespace Elektronik.Offline.Presenters
{
    public class SlamPackageInfoPresenter : RepaintablePackagePresenter
    {
        public EventInfoBanner info;
        public SlamMap map;

        private PacketPb m_packet;
        private SlamPoint[] m_objects;

        private IEnumerable<SlamPoint> pkg2pts(PacketPb packet)
        {
            switch (packet.DataCase)
            {
                case PacketPb.DataOneofCase.Points:
                    return packet.Points.Data.Select(p => 
                        new SlamPoint(p.Id, map.Points[p].position, color: default, message: p.Message));
                case PacketPb.DataOneofCase.Observations:
                    return packet.Observations.Data.Select(o =>
                        new SlamPoint(o.Point.Id, map.Observations[o].point.position, color: default, message: o.Point.Message));
                default:
                    return null;
            }
        }
        public override void Present(PacketPb packet)
        {
            if (!packet.Special)
                return;
            m_packet = packet;
            if (m_packet.Action == PacketPb.Types.ActionType.Info)
            {
                m_objects = pkg2pts(m_packet).ToArray();
            }
            m_presenter?.Present(packet);
            return;
        }
        public override void Clear() => info.Clear();
        public override void Repaint()
        {
            if (m_packet != null && m_packet.Special)
            {
                info.Clear();
                info.UpdateCommonInfo(m_packet.Message);
                if (m_packet.Action == PacketPb.Types.ActionType.Info)
                    info.UpdateExtraInfo(m_packet.DataCase.ToString(), m_objects);
            }
        }
    }
}
