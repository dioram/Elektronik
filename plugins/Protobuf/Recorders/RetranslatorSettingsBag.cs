using System;
using System.Linq;
using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.Recorders
{
    public class RetranslatorSettingsBag : SettingsBag
    {
        [CheckForEquals] public string Addresses = "127.0.0.1:5000;";

        public Action? StartRetranslation;
        public Action? StopRetranslation;

        public override ValidationResult Validate()
        {
            var isAllAddressesCorrect = Addresses
                    .Split(';')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .All(address => Uri.IsWellFormedUriString($"http://{address}", UriKind.Absolute));
            return isAllAddressesCorrect
                    ? ValidationResult.Succeeded
                    : ValidationResult.Failed("At least one of addresses is incorrect");
        }
    }
}