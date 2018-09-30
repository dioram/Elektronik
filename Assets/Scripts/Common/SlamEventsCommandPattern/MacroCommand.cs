using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class MacroCommand : ISlamEventCommand
    {
        protected List<ISlamEventCommand> m_commands;

        protected MacroCommand()
        {
            m_commands = new List<ISlamEventCommand>();
        }

        public virtual void Execute()
        {
            for (int i = 0; i < m_commands.Count; ++i)
            {
                m_commands[i].Execute();
            }
        }

        public virtual void UnExecute()
        {
            for (int i = m_commands.Count - 1; i >= 0; --i)
            {
                m_commands[i].UnExecute();
            }
        }
    }
}
