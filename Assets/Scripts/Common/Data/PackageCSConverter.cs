using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.Data
{
    public interface IPackageCSConverter
    {
        void Convert(ref Package srcEvent);
    }
}
