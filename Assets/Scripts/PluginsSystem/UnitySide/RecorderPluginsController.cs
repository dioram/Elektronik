﻿using System.IO;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.DataControllers;
using Elektronik.DataSources;
using Elektronik.UI;
using Elektronik.UI.Localization;
using Elektronik.UI.Windows;
using SimpleFileBrowser;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class RecorderPluginsController : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private DataSourcesController DataSourcesController;
        [SerializeField] private Button ToolbarButton;
        [SerializeField] private Window RecordersWindow;
        [SerializeField] private Button StartRecordingButton;
        [SerializeField] private InputWithBrowse InputWithBrowse;
        [SerializeField] private Sprite RecordImage;
        [SerializeField] private Sprite StopImage;
        [SerializeField] private CSConverter Converter;
        [SerializeField] private PluginWindowsManager WindowsManager;

        #endregion

        public static void Save(ISourceTreeNode node)
        {
            FileBrowser.SetFilters(false, _fileRecordersFactories.Select(f => f.Extension).ToArray());
            FileBrowser.ShowSaveDialog(path => Save(node, path[0]),
                                       () => { }, false, false, null,
                                       "Save to:".tr(), "Save".tr());
        }

        public void Toggle()
        {
            if (IsRecording)
            {
                _currentRecorder.Dispose();
            }
            else
            {
                RecordersWindow.Show();
            }
        }

        #region Unity events

        private void Awake()
        {
            _toolbarButtonImage = ToolbarButton.transform.Find("Image").GetComponent<Image>();
            _converter = Converter;
        }

        private void Start()
        {
            InitFileRecorders();
            InitCustomRecorders();
        }

        #endregion

        #region Private

        private Image _toolbarButtonImage;
        private static IFileRecorderPluginsFactory[] _fileRecordersFactories;
        private static ICSConverter _converter;
        private IDataRecorderPlugin _currentRecorder;
        private bool IsRecording => _currentRecorder != null;

        private void InitCustomRecorders()
        {
            var plugins = PluginsLoader.PluginFactories.Value
                .OfType<ICustomRecorderPluginsFactory>()
                .Select(f => (IDataRecorderPlugin)f.Start(Converter));
            foreach (var plugin in plugins)
            {
                WindowsManager.RegisterPlugin(plugin);
                DataSourcesController.AddRenderer(plugin);
            }
        }

        private void InitFileRecorders()
        {
            _fileRecordersFactories = PluginsLoader.PluginFactories.Value
                .OfType<IFileRecorderPluginsFactory>()
                .ToArray();

            if (_fileRecordersFactories.Length == 0)
            {
                Destroy(ToolbarButton.gameObject);
                Destroy(RecordersWindow.gameObject);
                Destroy(this);
                return;
            }

            InputWithBrowse.Filters = _fileRecordersFactories.Select(f => f.Extension).ToArray();

            StartRecordingButton.OnClickAsObservable()
                .Select(_ => InputWithBrowse.FilePath)
                .Select(GetRecorderByFileName)
                .Where(f => f != null)
                .Select(f => (IDataRecorderPlugin)f.Start(Converter))
                .Do(r => _currentRecorder = r)
                .Do(DataSourcesController.AddRenderer)
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

        private void OnRecorderDisposed()
        {
            _currentRecorder.OnDisposed -= OnRecorderDisposed;
            DataSourcesController.RemoveRenderer(_currentRecorder);
            _toolbarButtonImage.sprite = RecordImage;
            _currentRecorder = null;
        }

        private static IFileRecorderPluginsFactory GetRecorderByFileName(string path)
        {
            var res = _fileRecordersFactories.FirstOrDefault(f => f.Extension == Path.GetExtension(path));
            if (res is null) return null;
            res.Filename = path;
            return res;
        }

        static void Save(ISourceTreeNode node, string filename)
        {
            var factory = GetRecorderByFileName(filename);
            if (factory == null) return;
            var recorder = (IDataRecorderPlugin)factory.Start(_converter);
            node.AddConsumer(recorder);
            recorder.Dispose();
            node.RemoveConsumer(recorder);
        }

        #endregion
    }
}