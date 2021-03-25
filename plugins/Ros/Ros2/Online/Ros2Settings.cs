using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2Settings : SettingsBag
    {
        [CheckForEquals, Tooltip("Domain id")]
        public int DomainId = 0;

        public float Scale = 1;
    }
}
