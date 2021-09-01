using Elektronik.Commands;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline
{
    public class Frame
    {
        public int Timestamp;
        public bool IsSpecial;
        public ICommand? Command;

        public void Show()
        {
            Command?.Execute();
        }

        public void Rewind()
        {
            Command?.UnExecute();
        }

        public static Frame ParsePacket(PacketPb packetPb, DataParser<PacketPb> parsersChain)
        {
            return new Frame
            {
                Timestamp = packetPb.Timestamp,
                IsSpecial = packetPb.Special,
                Command = parsersChain.GetCommand(packetPb),
            };
        }
    }
}