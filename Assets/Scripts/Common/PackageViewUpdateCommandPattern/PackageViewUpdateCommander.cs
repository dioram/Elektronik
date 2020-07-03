using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Pb;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public abstract class PackageViewUpdateCommander : MonoBehaviour, IChainable<PackageViewUpdateCommander>
    {
        protected PackageViewUpdateCommander m_commander;
        public IChainable<PackageViewUpdateCommander> SetSuccessor(IChainable<PackageViewUpdateCommander> commander) => m_commander = commander as PackageViewUpdateCommander;
        public virtual void GetCommands(PacketPb pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            m_commander?.GetCommands(pkg, in commands);
        }

    }
}
