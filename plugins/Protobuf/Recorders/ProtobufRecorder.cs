﻿using System;
using System.IO;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Plugins.Common;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Google.Protobuf;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRecorder : FileRecorderPluginBase
    {
        public const uint Marker = 0xDEADBEEF;
        
        public ProtobufRecorder(string filename) : base(filename)
        {
            _file = File.OpenWrite(filename);
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamPoint> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPoint> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamPoint> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf().WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamLine> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamLine> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamLine> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf().WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SimpleLine> e)
        {
            // Can't record this type. Do nothing.
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SimpleLine> e)
        {
            // Can't record this type. Do nothing.
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SimpleLine> e)
        {
            // Can't record this type. Do nothing.
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamObservation> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamObservation> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamObservation> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf().WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamTrackedObject> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamTrackedObject> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamTrackedObject> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf().WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamPlane> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPlane> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf(_converter).WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<SlamPlane> e)
        {
            if (IsDisposed) return;
            lock (_file)
            {
                e.ToProtobuf().WriteDelimitedTo(_file);
                _amountOfPackets++;
            }
        }

        public override void Dispose()
        {
            lock (_file)
            {
                _file.Write(BitConverter.GetBytes(Marker), 0, 4);
                _file.Write(BitConverter.GetBytes(_amountOfPackets), 0, 4);
                _file.Close();
            }
            base.Dispose();
        }

        #region Private

        private readonly FileStream _file;
        private int _amountOfPackets = 0;
        private readonly ICSConverter _converter = new RightHandToLeftHandConverter();

        #endregion
    }
}