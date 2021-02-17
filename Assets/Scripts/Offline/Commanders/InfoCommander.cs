using System.Collections.Generic;
using Elektronik.Common.Commands;
using Elektronik.Common.Data.Pb;

namespace Elektronik.Offline.Commanders
{
    public class InfoCommander : Commander
    {
        public class InfoCommand : ICommand
        {
            public void Execute() { }

            public void UnExecute() { }
        }
        
        public override void GetCommands(PacketPb packet, in LinkedList<ICommand> commands)
        {
            commands.AddLast(new InfoCommand());

            base.GetCommands(packet, commands);
        }
    }
}