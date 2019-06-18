using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common
{
    public interface IChainable<T>
    {
        IChainable<T> SetSuccessor(IChainable<T> link);
    }
}
