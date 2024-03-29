﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using Grpc.Core;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRetranslator : ProtobufRecorderBase, IDataRecorderPlugin
    {
        public ProtobufRetranslator()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
        }
        
        #region IDataRecorderPlugin

        public void Start()
        {
            _clients = _settings.Addresses
                    .Split(';')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(address => new Channel(address, ChannelCredentials.Insecure))
                    .Select(channel => new MapsManagerPb.MapsManagerPbClient(channel))
                    .ToList();
        }

        public void Stop()
        {
            // Do nothing
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        public string DisplayName { get; } = "Protobuf retranslator";
        public string Description { get; } = "Allows Elektronik to transmit data from one instance to another";
        public SettingsBag Settings
        {
            get => _settings;
            set => _settings = (AddressesSettingsBag) value;
        }

        public ISettingsHistory SettingsHistory { get; } =
            new SettingsHistory<AddressesSettingsBag>($"{nameof(ProtobufRetranslator)}.json");

        public void StartRecording()
        {
            // Do nothing
        }

        public void StopRecording()
        {
            // Do nothing
        }

        public void OnAdded<TCloudItem>(string topicName, IList<TCloudItem> args) 
            where TCloudItem : struct, ICloudItem
        {
            var packet = CreateAddedPacket(args, GetTimestamp(), Converter);
            foreach (var client in _clients)
            {
                client.HandleAsync(packet);
            }
        }

        public void OnUpdated<TCloudItem>(string topicName, IList<TCloudItem> args) 
            where TCloudItem : struct, ICloudItem
        {
            var packet = CreateUpdatedPacket(args, GetTimestamp(), Converter);
            foreach (var client in _clients)
            {
                client.HandleAsync(packet);
            }
        }

        public void OnRemoved<TCloudItem>(string topicName, IList<int> args) 
            where TCloudItem : struct, ICloudItem
        {
            var packet = CreateRemovedPacket<TCloudItem>(args, GetTimestamp());
            foreach (var client in _clients)
            {
                client.HandleAsync(packet);
            }
        }

        public void OnConnectionsUpdated<TCloudItem>(string topicName, IList<(int id1, int id2)> items)
                where TCloudItem : struct, ICloudItem
        {
            var packet = CreateConnectionsUpdatedPacket<TCloudItem>(items, GetTimestamp());
            foreach (var client in _clients)
            {
                client.HandleAsync(packet);
            }
        }

        public void OnConnectionsRemoved<TCloudItem>(string topicName, IList<(int id1, int id2)> items)
                where TCloudItem : struct, ICloudItem
        {
            var packet = CreateConnectionsRemovedPacket<TCloudItem>(items, GetTimestamp());
            foreach (var client in _clients)
            {
                client.HandleAsync(packet);
            }
        }

        public bool StartsFromSceneLoading { get; } = true;

        public string Extension { get; } = "";
        public string FileName { get; set; }
        public ICSConverter Converter { get; set; }

        #endregion

        #region Private

        private int GetTimestamp()
        {
            return (int)(DateTime.Now - _initTime).TotalMilliseconds;
        }

        private AddressesSettingsBag _settings = new AddressesSettingsBag();
        private List<MapsManagerPb.MapsManagerPbClient> _clients = new List<MapsManagerPb.MapsManagerPbClient>();
        private readonly DateTime _initTime = DateTime.Now;

        #endregion
    }
}