using System;
using System.Collections.Generic;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.Settings
{
    [Serializable]
    public class SelectedPlugins : SettingsBag
    {
        [CheckForEquals, SerializeField]
        public List<string> SelectedPluginsNames = new List<string>();
    }
}