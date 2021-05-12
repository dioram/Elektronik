using System;
using System.Collections.Generic;
using Elektronik.Data.PackageObjects;

namespace Elektronik.PluginsSystem
{
    public interface IDataRecorderPlugin : IElektronikPlugin
    {
        string Extension { get; }
        string FileName { get; set; }

        void StartRecording();
        void StopRecording();

        void OnAdded(string topicName, IList<ICloudItem> args);
        void OnUpdated(string topicName, IList<ICloudItem> args);
        void OnRemoved(string topicName, Type itemType, IList<int> args);
        void OnConnectionsUpdated(string topicName, IList<(int id1, int id2)> items);
        void OnConnectionsRemoved(string topicName, IList<(int id1, int id2)> items);
    }
}