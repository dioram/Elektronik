using Elektronik.Commands;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline.Parsers
{
    public class InfoParser : PackageParser
    {
        public class InfoCommand : ICommand
        {
            public void Execute() { }

            public void UnExecute() { }
        }
        
        public override ICommand GetCommand(PacketPb packet)
        {
            if (packet.Action == PacketPb.Types.ActionType.Info) return new InfoCommand();
            return base.GetCommand(packet);
        }
    }
}