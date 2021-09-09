using System;
using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.OnlineBuffered
{
    [Serializable]
    public class OnlineSettingsBag : SettingsBag
    {
        [CheckForEquals] public int ListeningPort = 5050;
    }
}