using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public abstract class PackageViewUpdateCommander : MonoBehaviour
    {
        protected PackageViewUpdateCommander m_commander;
        public PackageViewUpdateCommander SetSuccessor(PackageViewUpdateCommander commander) => m_commander = commander;
        public abstract LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg);
    }
}
