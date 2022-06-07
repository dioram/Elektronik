using System;
using Elektronik.PluginsSystem;
using Elektronik.UI;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Elektronik.DataControllers
{
    /// <summary> Controllers for player events such as play, pause, rewind etc. </summary>
    public class PlayerEventsController : MonoBehaviour
    {
        #region Editor fields

        /// <summary> Source of player events emitted by hotkeys. </summary>
        [SerializeField] [Tooltip("Source of player events emitted by hotkeys.")]
        private HotkeysRouter HotkeysRouter;

        /// <summary> Source of player events emitted by UI. </summary>
        [SerializeField] [Tooltip("Source of player events emitted by UI.")]
        private PlayerUIControls PlayerUIControls;

        #endregion

        /// <summary> Turns on or off UI panel with player buttons. </summary>
        /// <param name="state"></param>
        public void ActivateUI(bool state)
        {
            PlayerUIControls.gameObject.SetActive(state);
        }

        /// <summary> Current plugin that is controlled by this object. </summary>
        public IRewindableDataSource DataSourcePlugin
        {
            get => _dataSourcePlugin;
            set
            {
                if (_dataSourcePlugin == value) return;
                ConnectControlsToPlugin(value);
            }
        }

        #region Unity events

        private void Start()
        {
            if (HotkeysRouter == null)
            {
                throw new ArgumentNullException(nameof(HotkeysRouter), "Editor field is not set!");
            }

            if (PlayerUIControls == null)
            {
                throw new ArgumentNullException(nameof(PlayerUIControls), "Editor field is not set!");
            }

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
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    .Where(_ => DataSourcePlugin is IChangingSpeed)
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    .Subscribe(speed => ((IChangingSpeed)DataSourcePlugin).Speed = 1 / speed)
                    .AddTo(this);
        }

        #endregion

        #region Private

        [CanBeNull] private IRewindableDataSource _dataSourcePlugin;

        private void ConnectControlsToPlugin(IRewindableDataSource plugin)
        {
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

                if (plugin is null)
                {
                    _dataSourcePlugin = null;
                    PlayerUIControls.SetTimestamp("00:00:00.00");
                    PlayerUIControls.SetSliderMinValue(0);
                    PlayerUIControls.SetSliderMaxValue(1);
                    PlayerUIControls.SetSliderPosition(0);
                    return;
                }

                _dataSourcePlugin = plugin;
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

                // ReSharper disable once SuspiciousTypeConversion.Global
                PlayerUIControls.ActivateSpeedButtons(_dataSourcePlugin is IChangingSpeed);
        }

        #endregion
    }
}