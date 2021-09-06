using System;
using Elektronik.UI.Localization;

namespace Elektronik.Settings.Bags
{
    [Serializable]
    public class AddressPortSettingsBag : SettingsBag
    {
        [CheckForEquals] public string IPAddress = "127.0.0.1";

        [CheckForEquals] public int Port = 5050;

        public override ValidationResult Validate()
        {
            if (Port < 0 || Port >= 65536)
            {
                return ValidationResult.Failed("Port is not valid. Should be between 0 and 65563.".tr());
            }

            if (!Uri.IsWellFormedUriString($"http://{IPAddress}:{Port}", UriKind.Absolute))
            {
                return ValidationResult.Failed("IP address is not valid.".tr());
            }

            return ValidationResult.Succeeded;
        }
    }
}