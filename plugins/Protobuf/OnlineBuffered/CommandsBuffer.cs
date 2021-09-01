using System.Collections.Generic;
using Elektronik.Commands;

namespace Elektronik.Protobuf.OnlineBuffered
{
    public class CommandsBuffer
    {
        public CommandsBuffer(int bufferLength)
        {
            _commands.Capacity = bufferLength;
        }

        public void Push(ICommand command)
        {
            _commands.Add(command);
        }
        
        // public void Current()
        
        private readonly List<ICommand> _commands = new ();
    }
}