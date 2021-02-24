using System;
using Elektronik.Common.Settings;
using UnityEngine;

namespace Elektronik.Offline.Settings
{
    [Serializable]
    public class OfflineSettingsBag : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File), Tooltip("Choose file path:")]
        public string FilePath;

        [CheckForEquals, Path(PathAttribute.PathTypes.Directory), Tooltip("Choose path ot images:")]
        public string ImagePath;

        [Tooltip("Scale")]
        public float Scale;
    }
}