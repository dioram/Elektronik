using System.Collections.Generic;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;

namespace Elektronik.PluginsSystem
{
    public interface IDataRecorderPlugin : IElektronikPlugin
    {
        bool StartsFromSceneLoading { get; }
        
        string Extension { get; }
        string FileName { get; set; }
        ICSConverter Converter { get; set; }

        void StartRecording();
        void StopRecording();

        void OnAdded<TCloudItem>(string topicName, IList<TCloudItem> args) where TCloudItem : ICloudItem;
        void OnUpdated<TCloudItem>(string topicName, IList<TCloudItem> args) where TCloudItem : ICloudItem;
        void OnRemoved<TCloudItem>(string topicName, IList<int> args) where TCloudItem : ICloudItem;
        void OnConnectionsUpdated<TCloudItem>(string topicName, IList<(int id1, int id2)> items)
                where TCloudItem : ICloudItem;
        void OnConnectionsRemoved<TCloudItem>(string topicName, IList<(int id1, int id2)> items)
                where TCloudItem : ICloudItem;
    }
}