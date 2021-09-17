using Elektronik.PluginsSystem;
using Elektronik.UI;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Elektronik.DataControllers
{
    public class PlayerEventsController : MonoBehaviour
    {
        [SerializeField] private HotkeysRouter HotkeysRouter;
        [SerializeField] private PlayerUIControls PlayerUIControls;
        [CanBeNull] private IDataSourcePlugin _dataSourcePlugin;

        public IDataSourcePlugin DataSourcePlugin
        {
            get => _dataSourcePlugin;
            set
            {
                if (_dataSourcePlugin == value) return;
                PlayerUIControls.SetPausedState();

                if (!(_dataSourcePlugin is null))
                {
                    _dataSourcePlugin.OnPlayingStarted -= PlayerUIControls.SetPlayingState;
                    _dataSourcePlugin.OnPaused -= PlayerUIControls.SetPausedState;
                    _dataSourcePlugin.OnFinished -= PlayerUIControls.SetPausedState;
                    _dataSourcePlugin.OnPositionChanged -= PlayerUIControls.SetSliderPosition;
                    _dataSourcePlugin.OnAmountOfFramesChanged -= PlayerUIControls.SetSliderMaxValue;
                    _dataSourcePlugin.OnTimestampChanged -= PlayerUIControls.SetTimestamp;
                }

                if (value is null)
                {
                    _dataSourcePlugin = null;
                    PlayerUIControls.SetTimestamp("00:00:00.00");
                    PlayerUIControls.SetSliderMinValue(0);
                    PlayerUIControls.SetSliderMaxValue(1);
                    PlayerUIControls.SetSliderPosition(0);
                    return;
                }

                _dataSourcePlugin = value;
                PlayerUIControls.SetTimestamp(_dataSourcePlugin.Timestamp);
                PlayerUIControls.SetSliderMinValue(0);
                PlayerUIControls.SetSliderMaxValue(_dataSourcePlugin.AmountOfFrames);
                PlayerUIControls.SetSliderPosition(_dataSourcePlugin.Position);

                _dataSourcePlugin.OnPlayingStarted += PlayerUIControls.SetPlayingState;
                _dataSourcePlugin.OnPaused += PlayerUIControls.SetPausedState;
                _dataSourcePlugin.OnFinished += PlayerUIControls.SetPausedState;
                _dataSourcePlugin.OnPositionChanged += PlayerUIControls.SetSliderPosition;
                _dataSourcePlugin.OnAmountOfFramesChanged += PlayerUIControls.SetSliderMaxValue;
                _dataSourcePlugin.OnTimestampChanged += PlayerUIControls.SetTimestamp;
            }
        }

        #region Unity events

        private void Start()
        {
            new[] { HotkeysRouter.OnPlayPause, PlayerUIControls.OnPlayPause }
                    .Merge()
                    .Select(_ => DataSourcePlugin)
                    .Where(p => !(p is null))
                    .Subscribe(p =>
                    {
                        if (p.IsPlaying) p.Pause();
                        else p.Play();
                    })
                    .AddTo(this);

            new[] { HotkeysRouter.OnStop, PlayerUIControls.OnStop }
                    .Merge()
                    .Select(_ => DataSourcePlugin)
                    .Where(p => !(p is null))
                    .Subscribe(p => p.StopPlaying());

            new[] { HotkeysRouter.OnNextKeyFrame, PlayerUIControls.OnRewindForward }
                    .Merge()
                    .Select(_ => DataSourcePlugin)
                    .Where(p => !(p is null))
                    .Subscribe(p => p.NextKeyFrame());

            new[] { HotkeysRouter.OnPreviousKeyFrame, PlayerUIControls.OnRewindBackward }
                    .Merge()
                    .Select(_ => DataSourcePlugin)
                    .Where(p => !(p is null))
                    .Subscribe(p => p.PreviousKeyFrame());

            HotkeysRouter.OnNextFrame
                    .Select(_ => DataSourcePlugin)
                    .Where(p => !(p is null))
                    .Subscribe(p => p.NextFrame());

            HotkeysRouter.OnPreviousFrame
                    .Select(_ => DataSourcePlugin)
                    .Where(p => !(p is null))
                    .Subscribe(p => p.PreviousFrame());

            PlayerUIControls.OnPositionChanged
                    .Where(_ => !(DataSourcePlugin is null))
                    .Subscribe(pos => DataSourcePlugin.Position = (int)pos)
                    .AddTo(this);

            PlayerUIControls.OnSpeedChanged
                    .Where(_ => !(DataSourcePlugin is null))
                    .Subscribe(speed => DataSourcePlugin.Speed = 1 / speed)
                    .AddTo(this);
        }

        #endregion
    }
}