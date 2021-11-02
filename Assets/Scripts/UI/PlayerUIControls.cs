using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.UI.Buttons;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class PlayerUIControls : MonoBehaviour
    {
        #region Editor fields

        [Header("Buttons")] 
        [SerializeField] private Button StopButton;
        [SerializeField] private Button RewindForward;
        [SerializeField] private Button RewindBackward;
        [SerializeField] private Button SpeedPanelButton;

        [Header("Play/Pause")] 
        [SerializeField] private Button PlayPauseButton;
        [SerializeField] private Image PlayPauseImage;
        [SerializeField] private Sprite PlayImage;
        [SerializeField] private Sprite PauseImage;

        [Space] 
        [SerializeField] private Slider PositionSlider;
        [SerializeField] private TMP_Text TimestampLabel;

        [Header("Speed settings")] 
        [SerializeField] private Transform SpeedButtonsPanel;
        [SerializeField] private GameObject SpeedButtonPrefab;
        [SerializeField] private float[] Speeds = { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, };

        #endregion

        public IObservable<Unit> OnPlayPause { get; private set; }

        public IObservable<Unit> OnStop { get; private set; }

        public IObservable<Unit> OnRewindForward { get; private set; }

        public IObservable<Unit> OnRewindBackward { get; private set; }

        public IObservable<float> OnSpeedChanged { get; private set; }

        /// <remarks> This will be triggered only when user moves slider. </remarks>
        public IObservable<float> OnPositionChanged { get; private set; }

        public void SetPlayingState()
        {
            UniRxExtensions.StartOnMainThread(() => PlayPauseImage.sprite = PauseImage).Subscribe();
        }

        public void SetPausedState()
        {
            UniRxExtensions.StartOnMainThread(() => PlayPauseImage.sprite = PlayImage).Subscribe();
        }

        public void ActivateSpeedButtons(bool state)
        {
            if (state == false)
            {
                SpeedButtonsPanel.gameObject.SetActive(false);
                SpeedPanelButton.gameObject.SetActive(false);
            }
            else
            {
                SpeedPanelButton.gameObject.SetActive(true);
            }
        }

        /// <summary> Displays new position on slider. </summary>
        /// <remarks> This will not trigger <c>OnPositionChanged</c> </remarks>
        public void SetSliderPosition(float sliderValue)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                _settingUpPosition = true;
                if (sliderValue > PositionSlider.maxValue)
                {
                    PositionSlider.value = PositionSlider.maxValue;
                }
                else if (sliderValue < PositionSlider.minValue)
                {
                    PositionSlider.value = PositionSlider.minValue;
                }
                else
                {
                    PositionSlider.value = sliderValue;
                }
                _settingUpPosition = false;
            }).Subscribe();
        }

        public void SetSliderPosition(int sliderValue) => SetSliderPosition((float)sliderValue);

        public void SetTimestamp(string timestamp)
        {
            UniRxExtensions.StartOnMainThread(() => TimestampLabel.text = timestamp).Subscribe();
        }

        public void SetSliderMaxValue(float sliderMax)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                _settingUpPosition = true;
                if (PositionSlider.value > sliderMax)
                {
                    PositionSlider.value = sliderMax;
                }

                PositionSlider.maxValue = sliderMax;
                _settingUpPosition = false;
            }).Subscribe();
        }

        public void SetSliderMaxValue(int sliderMax) => SetSliderMaxValue((float)sliderMax);

        public void SetSliderMinValue(float sliderMin)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                _settingUpPosition = true;
                if (PositionSlider.value < sliderMin)
                {
                    PositionSlider.value = sliderMin;
                }

                PositionSlider.minValue = sliderMin;
                _settingUpPosition = false;
            }).Subscribe();
        }

        public void SetSliderMinValue(int sliderMin) => SetSliderMinValue((float)sliderMin);

        #region Unity events

        private void Awake()
        {
            CreateSpeedButtons();

            var sliderSubject = new Subject<float>();
            PositionSlider.OnValueChangedAsObservable()
                    .Skip(1) // OnValueChangedAsObservable emits default value. Skipping it.
                    .Where(_ => !_settingUpPosition)
                    .Subscribe(sliderSubject);
            OnPositionChanged = sliderSubject;

            OnPlayPause = PlayPauseButton.OnClickAsObservable();
            OnStop = StopButton.OnClickAsObservable();
            OnRewindForward = RewindForward.OnClickAsObservable();
            OnRewindBackward = RewindBackward.OnClickAsObservable();
        }

        #endregion

        #region Private

        private bool _settingUpPosition;

        private void CreateSpeedButtons()
        {
            var toggles = Speeds.Select(CreateSpeedButton).ToArray();
            var subject = new Subject<float>();
            toggles.Select(t => t.OnToggled)
                    .Merge()
                    .Where(pair => pair.isToggled)
                    .Do(pair => DisableOtherToggles(toggles, pair.button))
                    .Select(pair => float.Parse(pair.button.Text.Substring(1)))
                    .Subscribe(subject)
                    .AddTo(this);
            OnSpeedChanged = subject;
        }

        private static void DisableOtherToggles(IEnumerable<ToggleButton> buttons, ToggleButton toggledButton)
        {
            foreach (var button in buttons.Where(b => b != toggledButton))
            {
                button.SetToggled(false);
            }
        }

        private ToggleButton CreateSpeedButton(float speed)
        {
            var go = Instantiate(SpeedButtonPrefab, SpeedButtonsPanel);
            var toggle = go.GetComponent<ToggleButton>();
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (speed == 1) toggle.SetToggled(true);
            toggle.Setup($"x{speed}");
            return toggle;
        }

        #endregion
    }
}