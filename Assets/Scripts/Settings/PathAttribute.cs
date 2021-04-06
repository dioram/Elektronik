using System;

namespace Elektronik.Settings
{
    public class PathAttribute : Attribute
    {
        public enum PathTypes
        {
            Directory,
            File,
        }

        public readonly PathTypes PathType;

        public readonly string[] Extensions;

        public PathAttribute(PathTypes pathType, string[] extensions)
        {
            PathType = pathType;
            Extensions = extensions;
        }
    }
}