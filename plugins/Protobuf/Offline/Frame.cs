using Elektronik.Plugins.Common.Commands;
using Elektronik.Plugins.Common.Parsing;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline
{
    public class Frame
    {
        public int Timestamp;
        public bool IsSpecial;
        private ICommand? _command;

        public void Show()
        {
            _command?.Execute();
        }

        public void Rewind()
        {
            _command?.UnExecute();
        }

        public static Frame ParsePacket(PacketPb packetPb, DataParser<PacketPb>? parsersChain)
        {
            return new Frame
            {
                Timestamp = packetPb.Timestamp,
                IsSpecial = packetPb.Special,
                _command = parsersChain?.GetCommand(packetPb),
            };
        }
    }
}