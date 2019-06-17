using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common
{
    public abstract class RepaintablePackagePresenter : PackagePresenter
    {
        public abstract void Repaint();
        public abstract void Clear();
    }
}
