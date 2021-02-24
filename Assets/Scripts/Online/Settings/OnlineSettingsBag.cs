using System;
using Elektronik.Common.Settings;
using UnityEngine;

namespace Elektronik.Online.Settings
{
    [Serializable]
    public class OnlineSettingsBag : SettingsBag
    {
        [CheckForEquals, Tooltip("IP address")]
        public string IPAddress = "127.0.0.1";

        [CheckForEquals, Tooltip("Port")]
        public int Port = 5050;

        [Tooltip("Scale")]
        public float Scale = 10f;
    }
}