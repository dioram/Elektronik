﻿using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Rosbag2.Data;
using Elektronik.Rosbag2.Parsers;
using SQLite;
using UnityEngine;

namespace Elektronik.Rosbag2.Containers
{
    public class TrackedDBContainer : TrackedObjectsContainer, IDBContainer
    {
        public TrackedDBContainer(string displayName) : base(displayName)
        {
            
        }
        
        #region IDBContainer implementation
        
        public long Timestamp { get; private set; }
        
        public SQLiteConnection DBModel { get; set; }
        
        public Topic Topic { get; set; }
        public long[] ActualTimestamps { get; set; }

        public void ShowAt(long newTimestamp, bool rewind = false)
        {
            lock (this)
            {
                if (ActualTimestamps.Length == 0) return;
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
            var messages = DBModel
                    .Table<Message>()
                    .Where(m => m.Timestamp < Timestamp && m.TopicID == Topic.Id)
                    .OrderBy(m => m.Timestamp)
                    .ToList()
                    .Select(m => MessageParser.Parse(m, Topic).GetPose().ToUnity())
                    .ToList();

            var obj = new SlamTrackedObject(0, messages.Last().Item1, messages.Last().Item2);
            if (messages.Count == 0)
            {
                Add(obj);
                return;
            }
            
            var history = new SlamLine[messages.Count - 1];
            for (int i = 0; i < messages.Count - 1; i++)
            {
                history[i] = new SlamLine(new SlamPoint(i, messages[i].Item1, Color.black),
                                          new SlamPoint(i + 1, messages[i + 1].Item1, Color.black),
                                          i);
            }
            AddWithHistory(obj, history);
        }

        private void SetNewPos()
        {
            var message = DBModel
                    .Table<Message>()
                    .Where(m => m.Timestamp < Timestamp && m.TopicID == Topic.Id)
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefault();
            if (message == null)
            {
                Clear();
                return;
            }

            var (v, q) = MessageParser.Parse(message, Topic).GetPose().ToUnity();
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
            long time = Timestamp;
            int pos = _pos;
            if (newTimestamp > Timestamp)
            {
                for (int i = _pos; i < ActualTimestamps.Length; i++)
                {
                    if (ActualTimestamps[i] > newTimestamp) break;
                    pos = i;
                    time = ActualTimestamps[i];
                }
            }
            else
            {
                for (int i = _pos; i >= 0; i--)
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