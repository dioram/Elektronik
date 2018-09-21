using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    public class MacroCommand : ISlamEventCommand
    {
        protected List<ISlamEventCommand> m_commands;

        protected MacroCommand()
        {
            m_commands = new List<ISlamEventCommand>();
        }

        public void Execute()
        {
            foreach (var command in m_commands)
            {
                command.Execute();
            }
        }

        public void UnExecute()
        {
            foreach (var command in m_commands)
            {
                command.UnExecute();
            }
        }
    }
}
