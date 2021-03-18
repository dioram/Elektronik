using System;
using System.IO;
using UnityEngine;

namespace Elektronik.Settings
{
    [Serializable]
    public class AddressPortScaleSettingsBag : SettingsBag
    {
        [CheckForEquals, Tooltip("IP address")]
        public string IPAddress = "127.0.0.1";

        [CheckForEquals]
        public int Port = 5050;

        public float Scale = 10f;
    }
}