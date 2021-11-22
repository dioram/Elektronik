﻿using System;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataControllers;
using Elektronik.Input;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI.Buttons;
using Elektronik.UI.Windows;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.UI
{
    public class HotkeysRouter : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private DataSourcesController DataSourcesController;
        [SerializeField] private VrController VrController;
        [SerializeField] private GpuMeshRendererComponent MeshRenderer;
        [SerializeField] private RecorderPluginsController RecorderPluginsController;
        [Header("Windows")] [SerializeField] private Window HelpWindow;
        [SerializeField] private Window SourceTreeWindow;
        [SerializeField] private Window SceneToolsWindow;
        [SerializeField] private Window AnalyticsToolsWindow;
        [SerializeField] private Window ConnectionsWindow;
        [Header("Buttons")] [SerializeField] private ChangingButton ToggleAxisButton;
        [Space] [SerializeField] private TMP_Dropdown GridSelector;

        #endregion

        public IObservable<Unit> OnPlayPause { get; private set; }
        public IObservable<Unit> OnStop { get; private set; }
        public IObservable<Unit> OnNextKeyFrame { get; private set; }
        public IObservable<Unit> OnPreviousKeyFrame { get; private set; }
        public IObservable<Unit> OnNextFrame { get; private set; }
        public IObservable<Unit> OnPreviousFrame { get; private set; }

        public void Awake()
        {
            var hotkeys = new CameraControls().Hotkeys;
            hotkeys.Enable();

            OnPlayPause = hotkeys.PlayPause.PerformedAsObservable().Select(_ => Unit.Default);
            OnStop = hotkeys.Stop.PerformedAsObservable().Select(_ => Unit.Default);
            OnNextKeyFrame = hotkeys.RewindForward.PerformedAsObservable().Select(_ => Unit.Default);
            OnPreviousKeyFrame = hotkeys.RewindBackward.PerformedAsObservable().Select(_ => Unit.Default);
            OnNextFrame = hotkeys.OneFrameForward.PerformedAsObservable().Select(_ => Unit.Default);
            OnPreviousFrame = hotkeys.OneFrameBackward.PerformedAsObservable().Select(_ => Unit.Default);

            hotkeys.Help.PerformedAsObservable()
                    .Subscribe(_ => HelpWindow.Show())
                    .AddTo(this);
            hotkeys.ShowSourceTree.PerformedAsObservable()
                    .Subscribe(_ => SourceTreeWindow.Show())
                    .AddTo(this);
            hotkeys.ShowSceneTools.PerformedAsObservable()
                    .Subscribe(_ => SceneToolsWindow.Show())
                    .AddTo(this);
            hotkeys.ShowAnalyticsTools.PerformedAsObservable()
                    .Subscribe(_ => AnalyticsToolsWindow.Show())
                    .AddTo(this);
            hotkeys.Open.PerformedAsObservable()
                    .Subscribe(_ => ConnectionsWindow.Show())
                    .AddTo(this);

            hotkeys.GoToVR.PerformedAsObservable()
                    .Where(_ => VrController.IsVrEnabled)
                    .Subscribe(_ => VrController.ToggleVrMode())
                    .AddTo(this);
            hotkeys.VRHelp.PerformedAsObservable()
                    .Where(_ => VrController.IsVrEnabled)
                    .Subscribe(_ => VrController.ToggleVrHelpMenu())
                    .AddTo(this);
            hotkeys.TakeSnapshot.PerformedAsObservable()
                    .Subscribe(_ => DataSourcesController.TakeSnapshot())
                    .AddTo(this);
            hotkeys.LoadSnapshot.PerformedAsObservable()
                    .Subscribe(_ => DataSourcesController.LoadSnapshot())
                    .AddTo(this);

            hotkeys.ToggleAxis.PerformedAsObservable()
                    .Subscribe(_ => ToggleAxisButton.Toggle())
                    .AddTo(this);
            hotkeys.SwitchMeshShader.PerformedAsObservable()
                    .Subscribe(_ => MeshRenderer.ShaderId++)
                    .AddTo(this);
            hotkeys.StartRecording.PerformedAsObservable()
                    .Where(_ => !RecorderPluginsController.IsRecording)
                    .Subscribe(_ => RecorderPluginsController.ShowWindow())
                    .AddTo(this);
            hotkeys.StartRecording.PerformedAsObservable()
                    .Where(_ => RecorderPluginsController.IsRecording)
                    .Subscribe(_ => RecorderPluginsController.StopRecording())
                    .AddTo(this);

            hotkeys.ToggleGrid.PerformedAsObservable()
                    .Subscribe(_ => GridSelector.value = (GridSelector.value + 1) % GridSelector.options.Count)
                    .AddTo(this);
        }
    }
}