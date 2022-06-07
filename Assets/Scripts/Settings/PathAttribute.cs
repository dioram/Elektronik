using System;
using JetBrains.Annotations;

namespace Elektronik.Settings
{
    /// <summary> Marks that target string settings field is path to some file or directory. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [BaseTypeRequired(typeof(string))]
    public class PathAttribute : Attribute
    {
        public enum PathTypes
        {
            Directory,
            File,
        }

        public readonly PathTypes PathType;

        /// <summary> Filtered file extensions. </summary>
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