using System.IO;
using System.Linq;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.UI;
using Elektronik.UI.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class RecorderPluginsController : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private DataSourcesManager DataSourcesManager;
        [SerializeField] private Button ToolbarButton;
        [SerializeField] private Window RecordersWindow;
        [SerializeField] private Button StartRecordingButton;
        [SerializeField] private InputWithBrowse FileBrowser;
        [SerializeField] private Sprite RecordImage;
        [SerializeField] private Sprite StopImage;
        [SerializeField] private CSConverter Converter;

        #endregion

        #region Unity events

        private void Awake()
        {
            _toolbarButtonImage = ToolbarButton.transform.Find("Image").GetComponent<Image>();
        }

        private void Start()
        {
            _recordersFactories = PluginsLoader.Instance.PluginFactories
                    .OfType<IFileRecorderPluginsFactory>()
                    .ToArray();

            if (_recordersFactories.Length == 0)
            {
                Destroy(ToolbarButton.gameObject);
                Destroy(RecordersWindow.gameObject);
                Destroy(this);
                return;
            }

            FileBrowser.Filters = _recordersFactories.Select(f => f.Extension).ToArray();

            StartRecordingButton.OnClickAsObservable()
                    .Select(_ => FileBrowser.FilePath)
                    .Select(GetRecorderByFileName)
                    .Where(f => f != null)
                    .Select(f => (IDataRecorderPlugin)f.Start(Converter))
                    .Do(r => _currentRecorder = r)
                    .Do(DataSourcesManager.AddRenderer)
                    .Do(_ => _currentRecorder.OnDisposed += OnRecorderDisposed)
                    .Do(_ => RecordersWindow.Hide())
                    .Subscribe(_ => _toolbarButtonImage.sprite = StopImage)
                    .AddTo(this);

            ToolbarButton.OnClickAsObservable()
                    .Where(_ => IsRecording)
                    .Subscribe(_ => _currentRecorder.Dispose())
                    .AddTo(this);

            ToolbarButton.OnClickAsObservable()
                    .Where(_ => !IsRecording)
                    .Subscribe(_ => RecordersWindow.Show())
                    .AddTo(this);
        }

        #endregion

        #region Private

        private Image _toolbarButtonImage;
        private IFileRecorderPluginsFactory[] _recordersFactories;
        private IDataRecorderPlugin _currentRecorder;
        private bool IsRecording => _currentRecorder != null;

        private void OnRecorderDisposed()
        {
            _currentRecorder.OnDisposed -= OnRecorderDisposed;
            DataSourcesManager.RemoveRenderer(_currentRecorder);
            _toolbarButtonImage.sprite = RecordImage;
            _currentRecorder = null;
        }

        private IFileRecorderPluginsFactory GetRecorderByFileName(string path)
        {
            var res = _recordersFactories.FirstOrDefault(f => f.Extension == Path.GetExtension(path));
            if (res is null) return null;
            res.Filename = path;
            return res;
        }

        #endregion
    }
}