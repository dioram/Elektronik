using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Containers
{
    public class InvalidSlamContainerOperationException : Exception
    {
        public InvalidSlamContainerOperationException(string message) : base(message)
        {
        }
    }
}
