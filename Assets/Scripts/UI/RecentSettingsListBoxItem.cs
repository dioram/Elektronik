﻿using System.Globalization;
using Elektronik.Settings;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class RecentSettingsListBoxItem : ListBoxItem
    {
        public Text DataLabel;
        public Text DateTimeLabel;
        private SettingsBag _data;

        public SettingsBag Data
        {
            get => _data;
            set
            {
                _data = value;
                DataLabel.text = _data.ToString();
                DateTimeLabel.text = _data.ModificationTime.ToString(CultureInfo.CurrentCulture);
            }
        }
    }
}