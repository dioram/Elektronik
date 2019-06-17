using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public class LambdaCommand : IPackageViewUpdateCommand
    {
        private Action m_execute;
        private Action m_unexecute;

        public LambdaCommand(Action execute, Action unexecute)
        {
            m_execute = execute;
            m_unexecute = unexecute;
        }

        public void Execute() => m_execute();

        public void UnExecute() => m_unexecute();
    }
}
