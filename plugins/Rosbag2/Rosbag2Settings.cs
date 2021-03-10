using System;
using System.IO;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.Rosbag2
{
    [Serializable]
    public class Rosbag2Settings : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File), Tooltip("Path to bag")]
        public string DirPath;

        public override bool Validate()
        {
            return File.Exists(DirPath);
        }
    }
}