using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public class SelectedPlugins : SettingsBag
    {
        [CheckForEquals, SerializeField]
        public List<string> SelectedPluginsNames = new List<string>();
    }
}