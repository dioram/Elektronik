using System;

namespace Elektronik.Containers
{
    public class InvalidSlamContainerOperationException : Exception
    {
        public InvalidSlamContainerOperationException(string message) : base(message)
        {
        }
    }
}
