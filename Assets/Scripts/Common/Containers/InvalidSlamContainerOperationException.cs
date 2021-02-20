using System;

namespace Elektronik.Common.Containers
{
    public class InvalidSlamContainerOperationException : Exception
    {
        public InvalidSlamContainerOperationException(string message) : base(message)
        {
        }
    }
}
