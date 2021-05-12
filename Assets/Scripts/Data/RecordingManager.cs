using System.Collections.Generic;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.Data
{
    public class RecordingManager : MonoBehaviour
    {
        public DataSourcesManager DataSourcesManager;

        private List<IDataRecorderPlugin> _recorders;

        private void Start()
        {
            Debug.Assert(DataSourcesManager != null, "DataSourcesManager != null");
            _recorders = PluginsLoader.Plugins.Value.OfType<IDataRecorderPlugin>().ToList();
        }

        private void OnDestroy()
        {
            _currentRecorder?.StopRecording();
        }

        public void StartRecording()
        {
            _currentRecorder = _recorders.FirstOrDefault();
            if (_currentRecorder is null) return;

            _currentRecorder.FileName = @$"D://test.{_currentRecorder.Extension}";
            DataSourcesManager.MapSourceTree((elem, topic) => !_currentRecorder.SubscribeOn(elem, topic));
            _currentRecorder.StartRecording();
        }

        public void StopRecording()
        {
            _currentRecorder?.StopRecording();
        }

        [CanBeNull] private IDataRecorderPlugin _currentRecorder;
    }
}