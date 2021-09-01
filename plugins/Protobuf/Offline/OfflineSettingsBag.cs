using System;
using System.IO;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.Protobuf.Offline
{
    [Serializable]
    public class OfflineSettingsBag : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File, new[] {".dat"}), Tooltip("Path to file")]
        public string FilePath = "";

        [CheckForEquals, Path(PathAttribute.PathTypes.Directory, new[] {""}), Tooltip("Path to images")]
        public string ImagePath = "";

        public override bool Validate()
        {
            return File.Exists(FilePath);
        }
    }
}