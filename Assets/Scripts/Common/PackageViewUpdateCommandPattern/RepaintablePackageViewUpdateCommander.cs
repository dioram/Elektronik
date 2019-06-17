using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public abstract class RepaintablePackageViewUpdateCommander : PackageViewUpdateCommander
    {
        public abstract void Repaint();
        public abstract void Clear();
    }
}
