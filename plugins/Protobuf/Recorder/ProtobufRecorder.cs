using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using Google.Protobuf;

namespace Elektronik.Protobuf.Recorder
{
    public class ProtobufRecorder : IDataRecorderPlugin
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

        public void OnAdded<TCloudItem>(string topicName, IList<TCloudItem> args) where TCloudItem : ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Timestamp = GetTimestamp(),
            };
            SetData(args, packet);
            packet.WriteDelimitedTo(_file);
        }

        public void OnUpdated<TCloudItem>(string topicName, IList<TCloudItem> args) where TCloudItem : ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Timestamp = GetTimestamp(),
            };
            SetData(args, packet);
            packet.WriteDelimitedTo(_file);
        }
        
        public void OnRemoved<TCloudItem>(string topicName, IList<int> args) where TCloudItem : ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Timestamp = GetTimestamp(),
            };
            SetData<TCloudItem>(args, packet);
            packet.WriteDelimitedTo(_file);
        }

        public void OnConnectionsUpdated<TCloudItem>(string topicName, IList<(int id1, int id2)> items) where TCloudItem : ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Timestamp = GetTimestamp(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            SetData<TCloudItem>(new List<int>(), packet);
            
            packet.Connections.Data.Add(items.Select(i => new ConnectionPb {Id1 = i.id1, Id2 = i.id2}));
            packet.WriteDelimitedTo(_file);
        }

        public void OnConnectionsRemoved<TCloudItem>(string topicName, IList<(int id1, int id2)> items) where TCloudItem : ICloudItem
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Timestamp = GetTimestamp(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
            };
            SetData<TCloudItem>(new List<int>(), packet);
            
            packet.Connections.Data.Add(items.Select(i => new ConnectionPb {Id1 = i.id1, Id2 = i.id2}));
            packet.WriteDelimitedTo(_file);
        }

        #region Private

        private bool _isRecording = false;
        private DateTime _recordingStart;
        private FileStream _file;
        private int _amountOfFrames = 0;

        private int GetTimestamp()
        {
            return (int)(DateTime.Now - _recordingStart).TotalMilliseconds;
        }

        private void SetData<TCloudItem>(IList<TCloudItem> items, PacketPb packet) where TCloudItem : ICloudItem
        {
            var first = items.FirstOrDefault();
            switch (first)
            {
            case SlamPoint _:
                packet.Points = new PacketPb.Types.Points();
                packet.Points.Data.AddRange(items.OfType<SlamPoint>().Select(s => s.ToProtobuf(Converter)));
                break;
            case SlamObservation _:
                packet.Observations = new PacketPb.Types.Observations();
                packet.Observations.Data.AddRange(items.OfType<SlamObservation>().Select(s => s.ToProtobuf(Converter)));
                break;
            case SlamTrackedObject _:
                packet.TrackedObjs = new PacketPb.Types.TrackedObjs();
                packet.TrackedObjs.Data.AddRange(items.OfType<SlamTrackedObject>().Select(s => s.ToProtobuf(Converter)));
                break;
            case SlamInfinitePlane _:
                packet.InfinitePlanes = new PacketPb.Types.InfinitePlanes();
                packet.InfinitePlanes.Data.AddRange(items.OfType<SlamInfinitePlane>().Select(s => s.ToProtobuf(Converter)));
                break;
            case SlamLine _:
                packet.Lines = new PacketPb.Types.Lines();
                packet.Lines.Data.AddRange(items.OfType<SlamLine>().Select(s => s.ToProtobuf(Converter)));
                break;
            }
        }

        private void SetData<TCloudItem>(IList<int> args, PacketPb packet) where TCloudItem : ICloudItem
        {
            if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamPoint)))
            {
                packet.Points = new PacketPb.Types.Points();
                packet.Points.Data.AddRange(args.Select(i => new PointPb {Id = i}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamObservation)))
            {
                packet.Observations = new PacketPb.Types.Observations();
                packet.Observations.Data.AddRange(args.Select(i => new ObservationPb {Point = new PointPb {Id = i}}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamTrackedObject)))
            {
                packet.TrackedObjs = new PacketPb.Types.TrackedObjs();
                packet.TrackedObjs.Data.AddRange(args.Select(i => new TrackedObjPb {Id = i}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamInfinitePlane)))
            {
                packet.InfinitePlanes = new PacketPb.Types.InfinitePlanes();
                packet.InfinitePlanes.Data.AddRange(args.Select(i => new InfinitePlanePb {Id = i}));
            }
            else if (typeof(TCloudItem).IsAssignableFrom(typeof(SlamLine)))
            {
                packet.Lines = new PacketPb.Types.Lines();
                packet.Lines.Data.AddRange(args.Select(i => new LinePb
                {
                    Pt1 = new PointPb {Id = 1},
                    Pt2 = new PointPb {Id = 2},
                }));
            }
        }

        #endregion
    }
}