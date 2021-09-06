using Elektronik.Settings;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2Settings : SettingsBag
    {
        [CheckForEquals, Tooltip("If you don't know what does it mean then it should be 0.")]
        public int DomainId = 0;
    }
}
