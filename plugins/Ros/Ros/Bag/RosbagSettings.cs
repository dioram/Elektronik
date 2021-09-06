using System.IO;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using Elektronik.UI.Localization;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagSettings : SettingsBag
    {
        [CheckForEquals, Path(new []{".bag"})]
        public string PathToBag = "";
        
        public override ValidationResult Validate()
        {
            return File.Exists(PathToBag) ? ValidationResult.Succeeded : ValidationResult.Failed("File not found".tr());
        }
    }
}