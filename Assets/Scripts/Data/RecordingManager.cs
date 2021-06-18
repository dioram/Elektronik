using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI.Localization;
using JetBrains.Annotations;
using SimpleFileBrowser;
using UnityEngine;

namespace Elektronik.Data
{
    public class RecordingManager : MonoBehaviour
    {
        public PluginsPlayer PluginsPlayer;
        public DataSourcesManager DataSourcesManager;

        private List<IDataRecorderPlugin> _recorders;

        private void Awake()
        {
            _recorders = PluginsPlayer.Plugins.OfType<IDataRecorderPlugin>().ToList();
            foreach (var recorder in _recorders.Where(r => r.StartsFromSceneLoading))
            {
                recorder.Converter = DataSourcesManager.Converter;
            }
            DataSourcesManager.OnSourceAdded += _ =>
            {
                DataSourcesManager.MapSourceTree((elem, topic) =>
                {
                    var subscribed = false;
                    foreach (var recorder in _recorders.Where(r => r.StartsFromSceneLoading))
                    {
                        subscribed = subscribed || recorder.SubscribeOn(elem, topic);
                    }
                    return !subscribed;
                });
            };
        }

        private void OnDestroy()
        {
            _currentRecorder?.StopRecording();
        }

        public void StartRecording()
        {
            FileBrowser.SetFilters(false, _recorders.Select(r => r.Extension).Where(s => !string.IsNullOrEmpty(s)));
            FileBrowser.ShowSaveDialog(path => Record(path[0]),
                                       () => { },
                                       false,
                                       false,
                                       "",
                                       TextLocalizationExtender.GetLocalizedString("Save record"),
                                       TextLocalizationExtender.GetLocalizedString("Save"));
        }

        public void StopRecording()
        {
            _currentRecorder?.StopRecording();
            _currentRecorder = null;
        }

        [CanBeNull] private IDataRecorderPlugin _currentRecorder;

        private void Record(string filename)
        {
            _currentRecorder = PluginsPlayer.Plugins
                    .OfType<IDataRecorderPlugin>()
                    .FirstOrDefault(r => filename.EndsWith(r.Extension));

            if (_currentRecorder is null) return;

            _currentRecorder.FileName = filename;
            _currentRecorder.Converter = DataSourcesManager.Converter;
            _currentRecorder.StartRecording();
            DataSourcesManager.MapSourceTree((elem, topic) => !_currentRecorder.SubscribeOn(elem, topic));
        }
    }
}