using System;
using System.IO;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.Protobuf.Offline
{
    [Serializable]
    public class OfflineSettingsBag : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File, new[] {".bag"}), Tooltip("Choose file path:")]
        public string FilePath;

        [CheckForEquals, Path(PathAttribute.PathTypes.Directory, new[] {""}), Tooltip("Choose path ot images:")]
        public string ImagePath;

        public float Scale = 10;

        public override bool Validate()
        {
            return File.Exists(FilePath);
        }
    }
}