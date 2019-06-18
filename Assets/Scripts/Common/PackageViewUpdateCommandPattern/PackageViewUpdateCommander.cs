using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public abstract class PackageViewUpdateCommander : MonoBehaviour, IChainable<PackageViewUpdateCommander>
    {
        protected PackageViewUpdateCommander m_commander;
        public IChainable<PackageViewUpdateCommander> SetSuccessor(IChainable<PackageViewUpdateCommander> commander) => m_commander = commander as PackageViewUpdateCommander;
        public abstract LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg);
    }
}
