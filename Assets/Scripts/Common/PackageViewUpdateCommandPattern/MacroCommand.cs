using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public class MacroCommand : IPackageViewUpdateCommand
    {
        protected IList<IPackageViewUpdateCommand> m_commands;

        public MacroCommand()
        {
            m_commands = new List<IPackageViewUpdateCommand>();
        }

        public virtual void Execute()
        {
            Debug.Log("[MacroCommand.Execute]");
            for (int i = 0; i < m_commands.Count; ++i)
            {
                m_commands[i].Execute();
            }
        }

        public virtual void UnExecute()
        {
            Debug.Log("[MacroCommand.UnExecute]");
            for (int i = m_commands.Count - 1; i >= 0; --i)
            {
                m_commands[i].UnExecute();
            }
        }
    }
}
