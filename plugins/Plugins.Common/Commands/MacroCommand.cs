using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Plugins.Common.Commands
{
    public class MacroCommand : ICommand
    {
        protected readonly IEnumerable<ICommand> Commands;

        public MacroCommand()
        {
            Commands = new List<ICommand>();
        }

        public MacroCommand(IEnumerable<ICommand> commands)
        {
            Commands = commands;
        }

        public virtual void Execute()
        {
            foreach (var command in Commands)
            {
                command.Execute();
            }
        }

        public virtual void UnExecute()
        {
            foreach (var command in Commands.Reverse())
            {
                command.UnExecute();
            }
        }
    }
}
