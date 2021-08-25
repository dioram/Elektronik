using System;
using System.Collections.Generic;
using System.IO;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using Google.Protobuf;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRecorder : ProtobufRecorderBase, IDataRecorderPlugin
    {
        public void Start()
        {
            // Do nothing
        }

        public void Stop()
        {
            // Do nothing
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        public string DisplayName { get; } = "Recorder to Protobuf";
        public string Description { get; } = "Records data to Protobuf file";
        public string Extension { get; } = ".dat";
        public string FileName { get; set; }
        public ICSConverter Converter { get; set; }
        public const uint Marker = 0xDEADBEEF;

        public SettingsBag Settings { get; set; } = new SettingsBag();
        public ISettingsHistory SettingsHistory { get; } = new FakeSettingsHistory();

        public void StartRecording()
        {
            _isRecording = true;
            _amountOfFrames = 0;
            _recordingStart = DateTime.Now;
            _file = File.OpenWrite(FileName);
        }

        public void StopRecording()
        {
            if (!_isRecording) return;
            _isRecording = false;
            _file.Write(BitConverter.GetBytes(Marker), 0, 4);
            _file.Write(BitConverter.GetBytes(_amountOfFrames), 0, 4);
            _file.Dispose();
        }

        public void OnAdded<TCloudItem>(string topicName, IList<TCloudItem> args) 
            where TCloudItem : struct, ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            CreateAddedPacket(args, GetTimestamp(), Converter).WriteDelimitedTo(_file);
        }

        public void OnUpdated<TCloudItem>(string topicName, IList<TCloudItem> args) 
            where TCloudItem : struct, ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            CreateUpdatedPacket(args, GetTimestamp(), Converter).WriteDelimitedTo(_file);
        }
        
        public void OnRemoved<TCloudItem>(string topicName, IList<int> args) 
            where TCloudItem : struct, ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            CreateRemovedPacket<TCloudItem>(args, GetTimestamp()).WriteDelimitedTo(_file);
        }

        public void OnConnectionsUpdated<TCloudItem>(string topicName, IList<(int id1, int id2)> items) 
            where TCloudItem : struct, ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            CreateConnectionsUpdatedPacket<TCloudItem>(items, GetTimestamp()).WriteDelimitedTo(_file);
        }

        public void OnConnectionsRemoved<TCloudItem>(string topicName, IList<(int id1, int id2)> items) 
            where TCloudItem : struct, ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            CreateConnectionsRemovedPacket<TCloudItem>(items, GetTimestamp()).WriteDelimitedTo(_file);
        }

        public bool StartsFromSceneLoading { get; } = false;

        #region Private

        private bool _isRecording = false;
        private DateTime _recordingStart;
        private FileStream _file;
        private int _amountOfFrames = 0;

        private int GetTimestamp()
        {
            return (int)(DateTime.Now - _recordingStart).TotalMilliseconds;
        }

        #endregion
    }
}