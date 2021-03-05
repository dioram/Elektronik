using System;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.Protobuf.Online
{
    [Serializable]
    public class OnlineSettingsBag : SettingsBag
    {
        [CheckForEquals, Tooltip("IP address")]
        public string IPAddress = "127.0.0.1";

        [CheckForEquals]
        public int Port = 5050;

        public float Scale = 10f;
    }
}