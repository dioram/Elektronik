using System;
using System.Globalization;
using Elektronik.Settings.Bags;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Windows.ConnectionsWindow
{
    [RequireComponent(typeof(Button))]
    public class RecentSettingsWidget : MonoBehaviour
    {
        [SerializeField] private TMP_Text DataLabel;
        [SerializeField] private TMP_Text DateTimeLabel;
        private SettingsBag _data;

        public IObservable<SettingsBag> OnSelected() => GetComponent<Button>().OnClickAsObservable().Select(_ => _data);

        public void Setup(SettingsBag data)
        {
            _data = data;
            DataLabel.text = _data.ToString();
            DateTimeLabel.text = _data.ModificationTime.ToString(CultureInfo.CurrentCulture);
        }
    }
}