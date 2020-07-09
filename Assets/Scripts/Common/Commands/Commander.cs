using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Pb;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Commands
{
    public abstract class Commander : MonoBehaviour, IChainable<Commander>
    {
        protected Commander m_commander;
        public IChainable<Commander> SetSuccessor(IChainable<Commander> commander) => m_commander = commander as Commander;
        public virtual void GetCommands(PacketPb pkg, in LinkedList<ICommand> commands)
        {
            m_commander?.GetCommands(pkg, in commands);
        }

    }
}
