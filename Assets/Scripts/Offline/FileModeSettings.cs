using System;

namespace Elektronik.Offline
{
    [Serializable]
    public class FileModeSettings : IComparable<FileModeSettings>
    {
        public string Path { get; set; }
        public float Scaling { get; set; }
        public DateTime Time { get; set; }
        public static FileModeSettings Current { get; set; }

        public int CompareTo(FileModeSettings other)
        {
            return Path.CompareTo(other.Path);
        }
    }
}
