using System;

namespace Elektronik.Common.Settings
{
    public class PathAttribute : Attribute
    {
        public enum PathTypes
        {
            Directory,
            File,
        }

        public readonly PathTypes PathType;

        public PathAttribute(PathTypes pathType)
        {
            PathType = pathType;
        }
    }
}