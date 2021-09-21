using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;
using UnityEngine;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class TrackedDBContainer : TrackedObjectsContainer, IDBContainer
    {
        public TrackedDBContainer(string displayName, List<SQLiteConnection> dbModels, Topic topic,
                                  List<long> actualTimestamps) :
                base(displayName)
        {
            DBModels = dbModels;
            Topic = topic;
            ActualTimestamps = actualTimestamps;
        }

        #region IDBContainer implementation

        public long Timestamp { get; private set; }

        public List<SQLiteConnection> DBModels { get; set; }

        public Topic Topic { get; set; }
        public List<long> ActualTimestamps { get; set; }

        public void ShowAt(long newTimestamp, bool rewind = false)
        {
            lock (this)
            {
                if (ActualTimestamps.Count == 0) return;
                var (time, pos) = GetValidTimestamp(newTimestamp);
                if (Timestamp == time) return;
                Timestamp = time;
                _pos = pos;

                if (rewind)
                {
                    Clear();
                    AddWithHistory();
                }
                else
                {
                    SetNewPos();
                }
            }
        }

        #endregion

        #region Private definitions

        private int _pos;

        private void AddWithHistory()
        {
            var messages = this.FindAllPreviousMessages()
                    .Select(m => MessageParser.Parse(m.Data, Topic.Type, true)!.GetPose()!.ToUnity())
                    .ToList();

            if (messages.Count == 0) return;

            var obj = new SlamTrackedObject(0, messages.Last().Item1, messages.Last().Item2);
            var history = new SimpleLine[messages.Count - 1];
            for (int i = 0; i < messages.Count - 1; i++)
            {
                history[i] = new SimpleLine(i, messages[i].pos, messages[i + 1].pos, Color.black);
            }

            AddWithHistory(obj, history);
        }

        private void SetNewPos()
        {
            var message = this.FindMessage();
            if (message == null)
            {
                Clear();
                return;
            }

            var (v, q) = MessageParser.Parse(message.Data, Topic.Type, true)!.GetPose()!.ToUnity();
            if (Count == 0)
            {
                Add(new SlamTrackedObject(0, v, q));
            }
            else
            {
                Update(new SlamTrackedObject(0, v, q));
            }
        }

        private (long time, int pos) GetValidTimestamp(long newTimestamp)
        {
            var time = Timestamp;
            var pos = _pos;
            if (newTimestamp > Timestamp)
            {
                for (var i = _pos; i < ActualTimestamps.Count; i++)
                {
                    if (ActualTimestamps[i] > newTimestamp) break;
                    pos = i;
                    time = ActualTimestamps[i];
                }
            }
            else
            {
                for (var i = _pos; i >= 0; i--)
                {
                    pos = i;
                    time = ActualTimestamps[i];
                    if (ActualTimestamps[i] < newTimestamp) break;
                }
            }

            return (time, pos);
        }

        #endregion
    }
}