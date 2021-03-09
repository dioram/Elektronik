using System;
using Elektronik.PluginsSystem;
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
        public Slider TimelineSlider;
        public Sprite PlayImage;
        public Sprite PauseImage;
        public Text Timestamp;

        private bool _isPlaying;
        private Image _playButtonImage;
        private IDataSourceOffline _dataSourceOffline;

        public void SetDataSource(IDataSourceOffline dataSourceOffline)
        {
            _dataSourceOffline = dataSourceOffline;
            Play += _dataSourceOffline.Play;
            Pause += _dataSourceOffline.Pause;
            Stop += _dataSourceOffline.StopPlaying;
            NextKeyFrame += _dataSourceOffline.NextKeyFrame;
            PreviousKeyFrame += _dataSourceOffline.PreviousKeyFrame;
            _dataSourceOffline.Finished += SetPausedState;
            TimelineSlider.OnValueChangedAsObservable()
                    .Select(v => (int) Mathf.Round((_dataSourceOffline.AmountOfFrames - 1) * v))
                    .Where(v => !(v == 0 && _dataSourceOffline.CurrentPosition == -1))
                    .Subscribe(value => _dataSourceOffline.CurrentPosition = value);
        }

        private void Awake()
        {
            _playButtonImage = PlayButton.transform.Find("Image").GetComponent<Image>();
            PlayButton.OnClickAsObservable()
                    .Subscribe(_ => PlayPause());
            StopButton.OnClickAsObservable()
                    .Do(_ => SetPausedState())
                    .Subscribe(_ => Stop?.Invoke());
            NextKeyFrameButton.OnClickAsObservable()
                    .Do(_ => SetPausedState())
                    .Subscribe(_ => NextKeyFrame?.Invoke());
            PreviousKeyFrameButton.OnClickAsObservable()
                    .Do(_ => SetPausedState())
                    .Subscribe(_ => PreviousKeyFrame?.Invoke());
        }

        private void Update()
        {
            var pos = _dataSourceOffline.CurrentPosition / (float) (_dataSourceOffline.AmountOfFrames - 1);
            if (!float.IsNaN(pos)) TimelineSlider.value = pos;
            Timestamp.text = $"{_dataSourceOffline.CurrentTimestamp}";
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
    }
}