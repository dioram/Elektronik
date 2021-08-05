using System;
using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.Online
{
    [Serializable]
    public class PortScaleSettingsBag : SettingsBag
    {
        [CheckForEquals]
        public int Port = 5050;

        public float Scale = 1;
    }
}