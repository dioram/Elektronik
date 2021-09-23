using System;
using Elektronik.Settings;

namespace Elektronik.RosPlugin.Ros.Online
{
    [Serializable]
    public class Ros1Settings: SettingsBag
    {
        [CheckForEquals]
        public string IPAddress = "127.0.0.1";

        [CheckForEquals]
        public int ListeningPort = 5050;

        public override ValidationResult Validate()
        {
            if (ListeningPort is < 0 or >= 65536)
            {
                return ValidationResult.Failed("Wrong port number. It should be between 0 and 65536");
            }

            if (!Uri.IsWellFormedUriString($"http://{IPAddress}:{ListeningPort}", UriKind.Absolute))
            {
                return ValidationResult.Failed("IP address is ill formed");
            }
            
            return  ValidationResult.Succeeded;
        }
    }
}