using System.IO;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagSettings : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File, new []{".bag"}), Tooltip("Path to file")]
        public string FilePath = "";

        public float Scale = 1f;
        
        public override bool Validate()
        {
            return File.Exists(FilePath);
        }
    }
}