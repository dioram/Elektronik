using Elektronik.Common.Data;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common
{
    public abstract class PackagePresenter : MonoBehaviour
    {
        protected PackagePresenter m_presenter;
        public PackagePresenter SetSuccessor(PackagePresenter presenter) => m_presenter = presenter;
        public abstract void Present(IPackage package);
    }
}
