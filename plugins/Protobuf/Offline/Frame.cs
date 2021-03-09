using Elektronik.Commands;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline
{
    public class Frame
    {
        public int Timestamp;
        public bool IsSpecial;
        public ICommand Command;
        public PacketPb Packet;

        public void Show()
        {
            Command.Execute();
        }

        public void Rewind()
        {
            Command.UnExecute();
        }

        public static Frame ParsePacket(PacketPb packetPb, DataParser<PacketPb> parsersChain)
        {
            return new Frame
            {
                Packet = packetPb,
                Timestamp = packetPb.Timestamp,
                IsSpecial = packetPb.Special,
                Command = parsersChain.GetCommand(packetPb),
            };
        }
    }
}