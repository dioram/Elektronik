using System;
using System.Collections.Generic;
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
        public event Action PreviousFrame;
        public event Action NextFrame;

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
        private IDataSourcePlugin _dataSourcePlugin;

        public readonly List<IObservable<Unit>> PlayPauseObservables = new List<IObservable<Unit>>();
        public readonly List<IObservable<Unit>> StopObservables = new List<IObservable<Unit>>();
        public readonly List<IObservable<Unit>> NextKeyFrameObservables = new List<IObservable<Unit>>();
        public readonly List<IObservable<Unit>> PreviousKeyFrameObservables = new List<IObservable<Unit>>();
        public readonly List<IObservable<Unit>> NextFrameObservables = new List<IObservable<Unit>>();
        public readonly List<IObservable<Unit>> PreviousFrameObservables = new List<IObservable<Unit>>();

        public void SetDataSource(IDataSourcePlugin dataSourcePlugin)
        {
            _dataSourcePlugin = dataSourcePlugin;
            Play += _dataSourcePlugin.Play;
            Pause += _dataSourcePlugin.Pause;
            Stop += () =>
            {
                Camera.main.transform.parent = null;
                _dataSourcePlugin.StopPlaying();
            };
            NextKeyFrame += _dataSourcePlugin.NextKeyFrame;
            PreviousKeyFrame += _dataSourcePlugin.PreviousKeyFrame;
            NextFrame += _dataSourcePlugin.NextFrame;
            PreviousFrame += _dataSourcePlugin.PreviousFrame;
            _dataSourcePlugin.Finished += SetPausedState;
            _dataSourcePlugin.Rewind += b => _isRewinding = b;
            TimelineSlider.OnTimelineChanged += f =>
            {
                if (_isRewinding) return;
                SetPausedState();
                _dataSourcePlugin.CurrentPosition =
                        (int)Mathf.Round((_dataSourcePlugin.AmountOfFrames - 1) * f);
            };
        }

        #region Unity events

        private void Awake()
        {
            _playButtonImage = PlayButton.transform.Find("Image").GetComponent<Image>();

            PlayPauseObservables.Add(PlayButton.OnClickAsObservable());
            StopObservables.Add(StopButton.OnClickAsObservable());
            NextKeyFrameObservables.Add(NextKeyFrameButton.OnClickAsObservable());
            PreviousKeyFrameObservables.Add(PreviousKeyFrameButton.OnClickAsObservable());
        }

        private void Start()
        {
            PlayPauseObservables.Merge()
                    .Subscribe(_ => PlayPause())
                    .AddTo(this);

            StopObservables.Merge()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => Stop?.Invoke())
                    .AddTo(this);
            
            NextKeyFrameObservables.Merge()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => NextKeyFrame?.Invoke())
                    .AddTo(this);
            
            PreviousKeyFrameObservables.Merge()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => PreviousKeyFrame?.Invoke())
                    .AddTo(this);
            
            NextFrameObservables.Merge()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => NextFrame?.Invoke())
                    .AddTo(this);
            
            PreviousFrameObservables.Merge()
                    .Do(_ => SetPausedState())
                    .Do(_ => UpdateControls(true))
                    .Subscribe(_ => PreviousFrame?.Invoke())
                    .AddTo(this);
        }

        private void Update()
        {
            if (_dataSourcePlugin is null) return;
            UpdateControls(_isPlaying);
        }

        #endregion

        #region Private

        private void UpdateControls(bool updateSlider)
        {
            if (updateSlider)
            {
                var pos = _dataSourcePlugin.CurrentPosition /
                        (float)(_dataSourcePlugin.AmountOfFrames - 1);
                if (!float.IsNaN(pos)) TimelineSlider.Value = pos;
            }

            Timestamp.text = $"{_dataSourcePlugin.CurrentTimestamp}";
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