using Elektronik.Common.Presenters;
using System.Collections.Generic;
using Elektronik.Common.Data.PackageObjects;
using System.Linq;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Containers;
using Elektronik.Common.Renderers;

namespace Elektronik.ProtobufPlugin.Offline.Presenters
{
    public class SlamDataInfoPresenter : DataPresenter
    {
        private IDataRenderer<(string info, string objectType, IEnumerable<SlamPoint> objects)> _info;
        private readonly IConnectableObjectsContainer<SlamPoint> _connectablePoints;
        private readonly IConnectableObjectsContainer<SlamObservation> _connectableObservations;


        public SlamDataInfoPresenter(IConnectableObjectsContainer<SlamPoint> connectablePoints,
                                     IConnectableObjectsContainer<SlamObservation> connectableObservations)
        {
            _connectablePoints = connectablePoints;
            _connectableObservations = connectableObservations;
        }

        private IEnumerable<SlamPoint> Pkg2Pts(PacketPb packet)
        {
            switch (packet.DataCase)
            {
            case PacketPb.DataOneofCase.Points:
                return packet.Points.Data.Select(p =>
                                                         new SlamPoint(p.Id,
                                                                       _connectablePoints[p.Id].Position,
                                                                       default,
                                                                       p.Message));
            case PacketPb.DataOneofCase.Observations:
                return packet.Observations.Data.Select(o =>
                                                               new SlamPoint(
                                                                   o.Point.Id,
                                                                   _connectableObservations[o.Point.Id].Point.Position,
                                                                   default,
                                                                   o.Point.Message));
            default:
                return null;
            }
        }

        public override void Present(object data)
        {
            if (data is Frame {IsSpecial: true} frame && _info != null)
            {
                _info.Clear();
                string objectsType = frame.Packet.Action == PacketPb.Types.ActionType.Info
                        ? frame.Packet.DataCase.ToString()
                        : null;
                IEnumerable<SlamPoint> objects = frame.Packet.Action == PacketPb.Types.ActionType.Info
                        ? Pkg2Pts(frame.Packet).ToArray()
                        : null;
                _info.Render((frame.Packet.Message, objectsType, objects));
            }

            base.Present(data);
        }

        public override void Clear()
        {
            _info?.Clear();
            base.Clear();
        }

        public override void SetRenderer(object dataRenderer)
        {
            if (dataRenderer 
                    is IDataRenderer<(string info, string objectType, IEnumerable<SlamPoint> objects)> renderer)
            {
                _info = renderer;
            }

            base.SetRenderer(dataRenderer);
        }
    }
}