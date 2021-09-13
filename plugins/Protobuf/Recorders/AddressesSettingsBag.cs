using System;
using System.Linq;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.Protobuf.Recorders
{
    public class AddressesSettingsBag : SettingsBag
    {
        [CheckForEquals, Tooltip("Addresses")]
        public string Addresses = "127.0.0.1:5000;";

        public override bool Validate()
        {
            return Addresses
                    .Split(';')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .All(address => Uri.IsWellFormedUriString($"htpp://{address}", UriKind.Absolute));
        }
    }
}