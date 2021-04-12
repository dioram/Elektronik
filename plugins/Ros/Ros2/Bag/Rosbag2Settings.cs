using System.IO;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag
{
    public class Rosbag2Settings : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File, new []{".db3"}), Tooltip("Path to file")]
        public string FilePath = "";

        public float Scale = 1f;
        
        public override bool Validate()
        {
            return File.Exists(FilePath);
        }
    }
}