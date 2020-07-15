using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Common.Commands
{
    public class MacroCommand : ICommand
    {
        protected IEnumerable<ICommand> m_commands;

        public MacroCommand()
        {
            m_commands = new List<ICommand>();
        }

        public MacroCommand(IEnumerable<ICommand> commands)
        {
            m_commands = commands;
        }

        public virtual void Execute()
        {
            foreach (var command in m_commands)
            {
                command.Execute();
            }
        }

        public virtual void UnExecute()
        {
            foreach (var command in m_commands.Reverse())
            {
                command.UnExecute();
            }
        }
    }
}
