using Elektronik.Offline.Loggers;
using Elektronik.Common.Presenters;
using System.Collections.Generic;
using Elektronik.Common.Data.PackageObjects;
using System.Linq;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Containers;

namespace Elektronik.Offline.Presenters
{
    public class SlamPackageInfoPresenter : RepaintablePackagePresenter
    {
        public EventInfoBanner info;
        public ConnectableObjectsContainer<SlamPoint> ConnectablePoints;
        public ConnectableObjectsContainer<SlamObservation> ConnectableObservations;

        private PacketPb _packet;
        private SlamPoint[] _objects;

        private IEnumerable<SlamPoint> Pkg2Pts(PacketPb packet)
        {
            switch (packet.DataCase)
            {
                case PacketPb.DataOneofCase.Points:
                    return packet.Points.Data.Select(p => 
                        new SlamPoint(p.Id, ConnectablePoints[p].Position, color: default, message: p.Message));
                case PacketPb.DataOneofCase.Observations:
                    return packet.Observations.Data.Select(o =>
                        new SlamPoint(o.Point.Id, ConnectableObservations[o].Point.Position, color: default, message: o.Point.Message));
                default:
                    return null;
            }
        }
        public override void Present(PacketPb packet)
        {
            if (!packet.Special)
                return;
            _packet = packet;
            if (_packet.Action == PacketPb.Types.ActionType.Info)
            {
                _objects = Pkg2Pts(_packet).ToArray();
            }

            if (Successor != null) Successor.Present(packet);
        }
        public override void Clear() => info.Clear();
        public override void Repaint()
        {
            if (_packet != null && _packet.Special)
            {
                info.Clear();
                info.UpdateCommonInfo(_packet.Message);
                if (_packet.Action == PacketPb.Types.ActionType.Info)
                    info.UpdateExtraInfo(_packet.DataCase.ToString(), _objects);
            }
        }
    }
}
