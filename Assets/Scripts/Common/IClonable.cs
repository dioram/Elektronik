using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common
{
    interface IClonable<T> where T : class
    {
        T Clone();
    }
}
