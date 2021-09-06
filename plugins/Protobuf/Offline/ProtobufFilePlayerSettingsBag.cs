using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.Protobuf.Offline
{
    public class ProtobufFilePlayerSettingsBag: SettingsBag
    {
        [Range(0, 9)]
        public int PlaybackSpeed = 8;
    }
}