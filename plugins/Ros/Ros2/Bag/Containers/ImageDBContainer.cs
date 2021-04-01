using System;
using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using Elektronik.UI.Windows;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class ImageDBContainer : ISourceTree, IDBContainer, IRendersToWindow
    {
        public ImageDBContainer(string displayName, SQLiteConnection dbModel, Topic topic, long[] actualTimestamps)
        {
            DisplayName = displayName;
            DBModel = dbModel;
            Topic = topic;
            ActualTimestamps = actualTimestamps;
        }

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; } = new ISourceTree[0];

        public bool IsActive { get; set; } = true;

        public void Clear()
        {
            if (_renderer is not null) _renderer.Clear();
        }

        public void SetRenderer(object renderer)
        {
            if (renderer is WindowsFactory factory)
            {
                factory.GetNewDataRenderer<ImageRenderer>(DisplayName, (imageRenderer, window) =>
                {
                    _renderer = imageRenderer;
                    _renderer.FlipVertically = true;
                    Window = window;
                });
            }
        }

        #endregion

        #region IDBContainer

        public long Timestamp { get; private set; } = -1;

        public SQLiteConnection DBModel { get; set; }

        public Topic Topic { get; set; }

        public long[] ActualTimestamps { get; set; }

        public void ShowAt(long newTimestamp, bool rewind = false)
        {
            if (ActualTimestamps.Length == 0) return;
            var (time, pos) = GetValidTimestamp(newTimestamp);
            if (Timestamp == time) return;
            Timestamp = time;
            _pos = pos;
            if (_renderer != null && _renderer.IsShowing) SetImage();
        }

        #endregion

        #region IRendersToWindow

        public Window Window { get; private set; }

        #endregion

        #region Private

        private ImageRenderer? _renderer;
        private int _pos;

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

        private void SetImage()
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

            var image = (MessageParser.Parse(message.Data, Topic.Type, true) as Image)!;
            if (_renderer is not null)
            {
                _renderer.Render(((int) image.width, (int) image.height, image.data,
                                  RosMessageConvertExtender.GetTextureFormat(image.encoding)));
            }
        }

        #endregion
    }
}