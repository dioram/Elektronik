using System;
using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.OnlineBuffered
{
    [Serializable]
    public class OnlineSettingsBag : SettingsBag
    {
        [CheckForEquals]
        public int Port = 5050;

        [CheckForEquals] public int FrameBufferLength = 1000;
    }
}