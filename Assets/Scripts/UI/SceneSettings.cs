using System.Linq;
using Elektronik.Settings;
using Elektronik.UI.Buttons;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

namespace Elektronik.UI
{
    /// <summary> Component for scene settings window. </summary>
    internal class SceneSettings : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private TMP_Dropdown GridDropdown;
        [SerializeField] private ChangingButton AxisButton;
        [SerializeField] private Slider PointSizeSlider;
        [SerializeField] private Slider DurationSlider;
        [SerializeField] private ColorPickerControl ColorPicker;
        [SerializeField] private Camera[] Cameras;

        #endregion

        #region Unity events

        private void Start()
        {
            _history = new SettingsHistory<SceneSettingsBag>($"{nameof(SceneSettingsBag)}.json", 1);
            _bag = _history.Recent.FirstOrDefault() as SceneSettingsBag;
            if (_bag == null) _bag = new SceneSettingsBag();
            else
            {
                GridDropdown.value = _bag.GridState;
                AxisButton.InitState(_bag.AxisState);
                DurationSlider.value = _bag.Duration;
                PointSizeSlider.value = _bag.PointSize;
                ColorPicker.CurrentColor = _bag.SceneColor;
            }

            _initialized = true;

            GridDropdown.onValueChanged.AddListener(i =>
            {
                _bag.GridState = i;
                SaveSettings();
            });
            AxisButton.OnStateChanged += i =>
            {
                _bag.AxisState = i;
                SaveSettings();
            };
            DurationSlider.OnValueChangedAsObservable().Do(i => _bag.Duration = (int) i).Subscribe(_ => SaveSettings());
            PointSizeSlider.OnValueChangedAsObservable().Do(i => _bag.PointSize = i).Subscribe(_ => SaveSettings());
            ColorPicker.onValueChanged.AddListener(color =>
            {
                if (!_initialized) return;
                _bag.SceneColor = color;
                foreach (var cam in Cameras)
                {
                    cam.backgroundColor = color;
                }
                SaveSettings();
            });
        }

        #endregion

        #region Private

        private SettingsHistory<SceneSettingsBag> _history;
        private SceneSettingsBag _bag;
        private bool _initialized = false;

        private void SaveSettings()
        {
            _history.Add(_bag.Clone());
            _history.Save();
        }

        #endregion
    }
}