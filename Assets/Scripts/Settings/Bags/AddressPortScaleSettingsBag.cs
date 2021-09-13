using System;
using UnityEngine;

namespace Elektronik.Settings.Bags
{
    [Serializable]
    public class AddressPortScaleSettingsBag : SettingsBag
    {
        [CheckForEquals, Tooltip("IP address")]
        public string IPAddress = "127.0.0.1";

        [CheckForEquals]
        public int Port = 5050;

        public float Scale = 10f;

        public override bool Validate()
        {
            return Uri.IsWellFormedUriString($"http://{IPAddress}:{Port}", UriKind.Absolute);
        }
    }
}