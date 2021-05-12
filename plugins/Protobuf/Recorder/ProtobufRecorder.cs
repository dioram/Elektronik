using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public string Extension { get; } = "dat";
        public string FileName { get; set; }
        public const uint Marker = 0xDEADBEEF;

        public SettingsBag Settings { get; set; }
        public ISettingsHistory SettingsHistory { get; }

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

        public void OnAdded(string topicName, IList<ICloudItem> args)
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

        public void OnUpdated(string topicName, IList<ICloudItem> args)
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

        public void OnRemoved(string topicName, Type itemType, IList<int> args)
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Timestamp = GetTimestamp(),
            };
            SetData(itemType, args, packet);
            packet.WriteDelimitedTo(_file);
        }

        public void OnConnectionsUpdated(string topicName, IList<(int id1, int id2)> items)
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Timestamp = GetTimestamp(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            packet.Connections.Data.Add(items.Select(i => new ConnectionPb {Id1 = i.id1, Id2 = i.id2}));
            packet.WriteDelimitedTo(_file);
        }

        public void OnConnectionsRemoved(string topicName, IList<(int id1, int id2)> items)
        {
            if (!_isRecording) return;
            _amountOfFrames++;
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Timestamp = GetTimestamp(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
            };
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

        private void SetData(IList<ICloudItem> items, PacketPb packet)
        {
            var first = items.FirstOrDefault();
            switch (first)
            {
            case SlamPoint p:
                packet.Points = new PacketPb.Types.Points();
                packet.Points.Data.AddRange(items.Select(i => (PointPb) (SlamPoint) i));
                break;
            case SlamObservation o:
                packet.Observations = new PacketPb.Types.Observations();
                packet.Observations.Data.AddRange(items.Select(i => (ObservationPb) (SlamObservation) i));
                break;
            case SlamTrackedObject t:
                packet.TrackedObjs = new PacketPb.Types.TrackedObjs();
                packet.TrackedObjs.Data.AddRange(items.Select(i => (TrackedObjPb) (SlamTrackedObject) i));
                break;
            case SlamInfinitePlane p:
                packet.InfinitePlanes = new PacketPb.Types.InfinitePlanes();
                packet.InfinitePlanes.Data.AddRange(items.Select(i => (InfinitePlanePb) (SlamInfinitePlane) i));
                break;
            case SlamLine l:
                packet.Lines = new PacketPb.Types.Lines();
                packet.Lines.Data.AddRange(items.Select(i => (LinePb) (SlamLine) i));
                break;
            }
        }

        private void SetData(Type itemType, IList<int> args, PacketPb packet)
        {
            if (itemType.IsAssignableFrom(typeof(SlamPoint)))
            {
                packet.Points = new PacketPb.Types.Points();
                packet.Points.Data.AddRange(args.Select(i => new PointPb {Id = i}));
            }
            else if (itemType.IsAssignableFrom(typeof(SlamObservation)))
            {
                packet.Observations = new PacketPb.Types.Observations();
                packet.Observations.Data.AddRange(args.Select(i => new ObservationPb {Point = new PointPb {Id = i}}));
            }
            else if (itemType.IsAssignableFrom(typeof(SlamTrackedObject)))
            {
                packet.TrackedObjs = new PacketPb.Types.TrackedObjs();
                packet.TrackedObjs.Data.AddRange(args.Select(i => new TrackedObjPb {Id = i}));
            }
            else if (itemType.IsAssignableFrom(typeof(SlamInfinitePlane)))
            {
                packet.InfinitePlanes = new PacketPb.Types.InfinitePlanes();
                packet.InfinitePlanes.Data.AddRange(args.Select(i => new InfinitePlanePb {Id = i}));
            }
            else if (itemType.IsAssignableFrom(typeof(SlamLine)))
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