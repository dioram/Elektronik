using System;
using System.IO;
using UnityEngine;

namespace Elektronik.Settings
{
    [Serializable]
    public class FileScaleSettingsBag : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File), Tooltip("Path to file")]
        public string FilePath = "";

        public float Scale = 1f;
        
        public override bool Validate()
        {
            return File.Exists(FilePath);
        }
    }
}