using System.Collections.Generic;

namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public class MacroCommand : IPackageViewUpdateCommand
    {
        protected IList<IPackageViewUpdateCommand> m_commands;

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
