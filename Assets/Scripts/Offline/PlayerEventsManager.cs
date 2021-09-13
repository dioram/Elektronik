using System;
using Elektronik.PluginsSystem;
using Elektronik.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Offline
{
    public class PlayerEventsManager : MonoBehaviour
    {
        public event Action Play;
        public event Action Pause;
        public event Action Stop;
        public event Action PreviousKeyFrame;
        public event Action NextKeyFrame;

        public Button PlayButton;
        public Button StopButton;
        public Button NextKeyFrameButton;
        public Button PreviousKeyFrameButton;
        public TimelineSlider TimelineSlider;
        public Sprite PlayImage;
        public Sprite PauseImage;
        public Text Timestamp;

        private bool _isRewinding;
        private bool _isPlaying;
        private Image _playButtonImage;
        private IDataSourcePluginOffline _dataSourcePluginOffline;

        public void SetDataSource(IDataSourcePluginOffline dataSourcePluginOffline)
        {
            _dataSourcePluginOffline = dataSourcePluginOffline;
            Play += _dataSourcePluginOffline.Play;
            Pause += _dataSourcePluginOffline.Pause;
            Stop += () =>
            {
                Camera.main.transform.parent = null;
                _dataSourcePluginOffline.StopPlaying();
            };
            NextKeyFrame += _dataSourcePluginOffline.NextKeyFrame;
            PreviousKeyFrame += _dataSourcePluginOffline.PreviousKeyFrame;
            _dataSourcePluginOffline.Finished += SetPausedState;
            _dataSourcePluginOffline.Rewind += b => _isRewinding = b;
            TimelineSlider.OnTimelineChanged += f =>
            {
                if (_isRewinding) return;
                SetPausedState();
                _dataSourcePluginOffline.CurrentPosition = (int) Mathf.Round((_dataSourcePluginOffline.AmountOfFrames - 1) * f);
            };
        }

        #region Unity events

        private void Awake()
        {
            _playButtonImage = PlayButton.transform.Find("Image").GetComponent<Image>();
            PlayButton.OnClickAsObservable()
                    .Subscribe(_ => PlayPause());
            StopButton.OnClickAsObservable()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => Stop?.Invoke());
            NextKeyFrameButton.OnClickAsObservable()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => NextKeyFrame?.Invoke());
            PreviousKeyFrameButton.OnClickAsObservable()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => PreviousKeyFrame?.Invoke());
        }

        private void Update()
        {
            UpdateControls(_isPlaying);
        }

        #endregion

        #region Private

        private void UpdateControls(bool updateSlider)
        {
            if (updateSlider)
            {
                var pos = _dataSourcePluginOffline.CurrentPosition / (float) (_dataSourcePluginOffline.AmountOfFrames - 1);
                if (!float.IsNaN(pos)) TimelineSlider.Value = pos;
            }
            Timestamp.text = $"{_dataSourcePluginOffline.CurrentTimestamp}";
        }

        private void PlayPause()
        {
            if (_isPlaying) SetPausedState();
            else SetPlayState();
        }

        private void SetPlayState()
        {
            if (_isPlaying) return;
            _playButtonImage.sprite = PauseImage;
            Play?.Invoke();
            _isPlaying = true;
        }

        private void SetPausedState()
        {
            if (!_isPlaying) return;
            _playButtonImage.sprite = PlayImage;
            Pause?.Invoke();
            _isPlaying = false;
        }

        #endregion
    }
}