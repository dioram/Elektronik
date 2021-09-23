using System;
using Elektronik.Settings;

namespace Elektronik.Protobuf.Online
{
    [Serializable]
    public class OnlineSettingsBag : SettingsBag
    {
        [CheckForEquals] public int ListeningPort = 5050;
    }
}