using Elektronik.Common.Settings;
using UnityEngine;

namespace Elektronik.RandomDataPlugin
{
    public class RandomSettingsBag : SettingsBag
    {
        [CheckForEquals, Tooltip("Placeholder")]
        public string JustString;
        
        [CheckForEquals, Tooltip("Placeholder"), Path(PathAttribute.PathTypes.Directory)]
        public string JustPath;

        [Tooltip("Scale")]
        public float Scale = 10;
    }
}