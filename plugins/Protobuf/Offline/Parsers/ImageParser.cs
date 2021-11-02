using Elektronik.Plugins.Common.Commands;
using Elektronik.Plugins.Common.Parsing;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.Presenters;

namespace Elektronik.Protobuf.Offline.Parsers
{
    public class ImageParser : DataParser<PacketPb>
    {
        private readonly ImagePresenter _presenter;
        private readonly string _imageDir;
        private ImageCommand? _lastCommand;
        
        public ImageParser(ImagePresenter presenter, string imageDir)
        {
            _presenter = presenter;
            _imageDir = imageDir;
        }
        
        public override ICommand? GetCommand(PacketPb packet)
        {
            if (packet.DataCase != PacketPb.DataOneofCase.Image) return base.GetCommand(packet);
            _lastCommand = new ImageCommand(_presenter, packet.ExtractImage(_imageDir), _lastCommand);
            return _lastCommand;
        }
    }
}