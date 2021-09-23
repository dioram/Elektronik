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

        private PathAttribute(PathTypes pathType, string[] extensions)
        {
            PathType = pathType;
            Extensions = extensions;
        }

        public PathAttribute(string[] extensions) : this(PathTypes.File, extensions)
        { }

        public PathAttribute(PathTypes pathType = PathTypes.File) : this(pathType, Array.Empty<string>())
        { }
    }
}