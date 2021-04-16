using Elektronik.Commands;
using Elektronik.Data.Converters;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Offline.Presenters;

namespace Elektronik.Protobuf.Offline.Parsers
{
    public class InfoParser : DataParser<PacketPb>
    {
        private readonly SlamDataInfoPresenter _presenter;
        
        public InfoParser(SlamDataInfoPresenter presenter)
        {
            _presenter = presenter;
        }
        
        public class InfoCommand : ICommand
        {
            private readonly SlamDataInfoPresenter _presenter;
            private readonly PacketPb _packet;
            private readonly ICSConverter _converter;

            public InfoCommand(SlamDataInfoPresenter presenter, PacketPb packet, ICSConverter converter)
            {
                _presenter = presenter;
                _packet = packet;
                _converter = converter;
            }
            
            public void Execute() { _presenter.Present(_packet, _converter); }

            public void UnExecute() { }
        }
        
        public override ICommand GetCommand(PacketPb packet)
        {
            if (packet.Action == PacketPb.Types.ActionType.Info) return new InfoCommand(_presenter, packet, Converter);
            return base.GetCommand(packet);
        }
    }
}