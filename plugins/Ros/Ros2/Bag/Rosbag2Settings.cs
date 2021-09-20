using System.IO;
using Elektronik.Settings;
using Elektronik.UI.Localization;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag
{
    public class Rosbag2Settings : SettingsBag
    {
        [CheckForEquals, Path(new[] { ".db3", ".yaml", ".yml" }), Tooltip("Path to metadata.yaml or *.db3 file.")]
        public string PathToFile = "";

        public override ValidationResult Validate()
        {
            return File.Exists(PathToFile)
                    ? ValidationResult.Succeeded
                    : ValidationResult.Failed("File not found".tr());
        }
    }
}