using System;
using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.Online
{
    [Serializable]
    public class OnlineSettingsBag : SettingsBag
    {
        [CheckForEquals]
        public int Port = 5050;
    }
}