using Elektronik.Clouds;
using Elektronik.Data;
using Elektronik.Input;
using Elektronik.Offline;
using Elektronik.Settings;
using Elektronik.UI.Windows;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.UI
{
    public class HotkeysRouter : MonoBehaviour
    {
        [SerializeField] private DataSourcesManager DataSourcesManager;
        [SerializeField] private VrController VrController;
        [SerializeField] private PlayerEventsManager EventsManager;
        [SerializeField] private GpuMeshRenderer MeshRenderer;
        [Header("Windows")] [SerializeField] private Window HelpWindow;
        [SerializeField] private Window SourceTreeWindow;
        [SerializeField] private Window SceneToolsWindow;
        [SerializeField] private Window AnalyticsToolsWindow;
        [SerializeField] private Window ConnectionsWindow;
        [Header("Buttons")] [SerializeField] private ChangingButton ToggleAxisButton;
        [SerializeField] private ChangingButton RecordingButton;
        [Space] [SerializeField] private TMP_Dropdown GridSelector;


        public void Awake()
        {
            var hotkeys = new CameraControls().Hotkeys;
            hotkeys.Enable();


            EventsManager.PlayPauseObservables.Add(hotkeys.PlayPause.PerformedAsObservable()
                                                           .Where(_ => ModeSelector.Mode == Mode.Offline)
                                                           .Select(_ => Unit.Default));
            EventsManager.StopObservables.Add(hotkeys.Stop.PerformedAsObservable()
                                                      .Where(_ => ModeSelector.Mode == Mode.Offline)
                                                      .Select(_ => Unit.Default));
            EventsManager.NextKeyFrameObservables.Add(hotkeys.RewindForward.PerformedAsObservable()
                                                              .Where(_ => ModeSelector.Mode == Mode.Offline)
                                                              .Select(_ => Unit.Default));
            EventsManager.PreviousKeyFrameObservables.Add(hotkeys.RewindBackward.PerformedAsObservable()
                                                                  .Where(_ => ModeSelector.Mode == Mode.Offline)
                                                                  .Select(_ => Unit.Default));
            EventsManager.NextFrameObservables.Add(hotkeys.OneFrameForward.PerformedAsObservable()
                                                           .Where(_ => ModeSelector.Mode == Mode.Offline)
                                                           .Select(_ => Unit.Default));
            EventsManager.PreviousFrameObservables.Add(hotkeys.OneFrameBackward.PerformedAsObservable()
                                                               .Where(_ => ModeSelector.Mode == Mode.Offline)
                                                               .Select(_ => Unit.Default));

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
                    .Subscribe(_ => DataSourcesManager.TakeSnapshot())
                    .AddTo(this);
            hotkeys.LoadSnapshot.PerformedAsObservable()
                    .Subscribe(_ => DataSourcesManager.LoadSnapshot())
                    .AddTo(this);
            hotkeys.ClearMap.PerformedAsObservable()
                    .Where(_ => ModeSelector.Mode == Mode.Online)
                    .Subscribe(_ => DataSourcesManager.ClearMap())
                    .AddTo(this);

            hotkeys.ToggleAxis.PerformedAsObservable()
                    .Subscribe(_ => ToggleAxisButton.Toggle())
                    .AddTo(this);
            hotkeys.ToggleMeshColor.PerformedAsObservable()
                    .Subscribe(_ => MeshRenderer.OverrideColors = !MeshRenderer.OverrideColors)
                    .AddTo(this);
            hotkeys.StartRecording.PerformedAsObservable()
                    .Subscribe(_ => RecordingButton.Toggle())
                    .AddTo(this);

            hotkeys.ToggleGrid.PerformedAsObservable()
                    .Subscribe(_ => GridSelector.value = (GridSelector.value + 1) % GridSelector.options.Count)
                    .AddTo(this);
        }
    }
}