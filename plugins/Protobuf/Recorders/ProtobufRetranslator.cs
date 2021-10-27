using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Plugins.Common;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Settings;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRetranslator : DataRecorderPluginBase
    {
        public ProtobufRetranslator(string displayName, Texture2D? logo)
        {
            DisplayName = displayName;
            Logo = logo;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            _typedSettings.StartRetranslation += StartRetranslation;
            _typedSettings.StopRetranslation += StopRetranslation;
        }
        
        #region IDataRecorderPlugin

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamPoint> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPoint> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamPoint> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf());
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamLine> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamLine> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamLine> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf());
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SimpleLine> e)
        {
            // Can't transmit this type. Do nothing.
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SimpleLine> e)
        {
            // Can't transmit this type. Do nothing.
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SimpleLine> e)
        {
            // Can't transmit this type. Do nothing.
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamObservation> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamObservation> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamObservation> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf());
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamTrackedObject> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamTrackedObject> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamTrackedObject> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf());
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamPlane> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPlane> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf(_converter));
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamPlane> e)
        {
            if (!_isTransmitting || _clients == null) return;
            SendPacket(e.ToProtobuf());
        }

        public override string DisplayName { get; }

        public override SettingsBag Settings => _typedSettings;
        public override Texture2D? Logo { get; }

        private readonly ICSConverter _converter = new ProtobufToUnityConverter();

        #endregion

        #region Private
        
        private List<MapsManagerPb.MapsManagerPbClient>? _clients;
        private readonly RetranslatorSettingsBag _typedSettings = new ();
        private bool _isTransmitting;

        private void StartRetranslation()
        { 
            if (_isTransmitting) return;
            _clients = _typedSettings.Addresses
                .Split(';')
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(address => new Channel(address, ChannelCredentials.Insecure))
                .Select(channel => new MapsManagerPb.MapsManagerPbClient(channel))
                .ToList();
            
            _isTransmitting = true;
        }

        private void StopRetranslation()
        {
            _isTransmitting = false;
            _clients = null;
        }

        private void SendPacket(PacketPb packet)
        {
            if (_clients == null) return;
            foreach (var client in _clients)
            {
                client.HandleAsync(packet);
            }
        }

        #endregion
    }
}