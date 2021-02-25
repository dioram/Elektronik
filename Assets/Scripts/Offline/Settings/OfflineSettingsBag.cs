using System;
using Elektronik.Common.Settings;
using UnityEngine;

namespace Elektronik.Offline.Settings
{
    [Serializable]
    public class OfflineSettingsBag : SettingsBag
    {
        [CheckForEquals, Path(PathAttribute.PathTypes.File), Tooltip("Choose file path:")]
        public string FilePath = @"C:\Users\User\RiderProjects\Elektronik-Tools-2.0\Examples\csharp\csharp.Tests\bin\Debug\netcoreapp3.1\TrackedObjsTests.dat";

        [CheckForEquals, Path(PathAttribute.PathTypes.Directory), Tooltip("Choose path ot images:")]
        public string ImagePath = @"C:\";

        [Tooltip("Scale")]
        public float Scale = 10;
    }
}