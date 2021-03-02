using Elektronik.Common.Commands;
using Elektronik.ProtobufPlugin.Common;
using Elektronik.ProtobufPlugin.Offline.Parsers;

namespace Elektronik.ProtobufPlugin.Offline
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

        public static Frame ParsePacket(PacketPb packetPb, PackageParser parsersChain)
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