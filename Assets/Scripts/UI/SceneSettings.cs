using System.Linq;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using HSVPicker;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class SceneSettings : MonoBehaviour
    {
        private SettingsHistory<SceneSettingsBag> _history;

        private SceneSettingsBag _bag;

        [SerializeField] private ChangingButton GridButton;
        [SerializeField] private ChangingButton AxisButton;
        [SerializeField] private Slider PointSizeSlider;
        [SerializeField] private Slider DurationSlider;
        [SerializeField] private ColorPicker ColorPicker;
        private bool _inited = false;

        private void Start()
        {
            _history = new SettingsHistory<SceneSettingsBag>($"{nameof(SceneSettingsBag)}.json", 1);
            _bag = _history.Recent.FirstOrDefault() as SceneSettingsBag;
            if (_bag == null) _bag = new SceneSettingsBag();
            else
            {
                GridButton.InitState(_bag.GridState);
                AxisButton.InitState(_bag.AxisState);
                DurationSlider.value = _bag.Duration;
                PointSizeSlider.value = _bag.PointSize;
                ColorPicker.CurrentColor = _bag.SceneColor;
            }

            _inited = true;

            GridButton.OnStateChanged += i =>
            {
                _bag.GridState = i;
                SaveSettings();
            };
            AxisButton.OnStateChanged += i =>
            {
                _bag.AxisState = i;
                SaveSettings();
            };
            DurationSlider.OnValueChangedAsObservable().Do(i => _bag.Duration = (int) i).Subscribe(_ => SaveSettings());
            PointSizeSlider.OnValueChangedAsObservable().Do(i => _bag.PointSize = i).Subscribe(_ => SaveSettings());
            ColorPicker.onValueChanged.AddListener(color =>
            {
                if (_inited)
                {
                    _bag.SceneColor = color;
                    SaveSettings();
                }
            });
        }

        private void SaveSettings()
        {
            _history.Add(_bag.Clone());
            _history.Save();
        }
    }
}