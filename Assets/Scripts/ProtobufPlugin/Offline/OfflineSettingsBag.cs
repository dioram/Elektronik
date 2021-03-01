using System;
using System.IO;
using Elektronik.Common.Settings;
using UnityEngine;

namespace Elektronik.ProtobufPlugin.Offline
{
    [Serializable]
    public class OfflineSettingsBag : SettingsBag
    {
        //TODO: Clear defaults
        [CheckForEquals, Path(PathAttribute.PathTypes.File), Tooltip("Choose file path:")]
        public string FilePath = @"C:\Users\User\RiderProjects\Elektronik-Tools-2.0\Examples\csharp\csharp.Tests\bin\Debug\netcoreapp3.1\TrackedObjsTests.dat";

        [CheckForEquals, Path(PathAttribute.PathTypes.Directory), Tooltip("Choose path ot images:")]
        public string ImagePath = @"C:\Users\User\RiderProjects\Elektronik-Tools-2.0\Examples\csharp\csharp.Tests\bin\Debug\netcoreapp3.1";

        [Tooltip("Scale")]
        public float Scale = 10;

        public override bool Validate()
        {
            return File.Exists(FilePath);
        }
    }
}