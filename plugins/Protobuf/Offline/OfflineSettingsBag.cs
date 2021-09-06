using System;
using System.IO;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using Elektronik.UI.Localization;

namespace Elektronik.Protobuf.Offline
{
    [Serializable]
    public class OfflineSettingsBag : SettingsBag
    {
        [CheckForEquals, Path(new[] { ".dat" })]
        public string? PathToFile = "";

        [CheckForEquals, Path(PathAttribute.PathTypes.Directory)]
        public string PathToImagesDirectory = "";

        public override ValidationResult Validate()
        {
            return File.Exists(PathToFile)
                    ? ValidationResult.Succeeded
                    : ValidationResult.Failed("File not found".tr());
        }
    }
}