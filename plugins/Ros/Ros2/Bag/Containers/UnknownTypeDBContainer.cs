using System.Collections.Generic;
using Elektronik.DataConsumers.Windows;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;
using Message = RosSharp.RosBridgeClient.Message;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class UnknownTypeDBContainer : DBContainerToWindow<Message, TableRenderer, string[]>
    {
        public UnknownTypeDBContainer(string displayName, List<SQLiteConnection> dbModels, Topic topic,
                                      List<long> actualTimestamps)
                : base(displayName, dbModels, topic, actualTimestamps)
        {
        }

        protected override string[] ToRenderType(Message message)
        {
            if (_isFirstMessage && Renderer is not null)
            {
                Renderer.SetHeader(RosMessageConvertExtender.GetMessagePropertyNames(message.GetType()));
                _isFirstMessage = false;
            }
            return message.GetData();
        }
        
        private bool _isFirstMessage = true;
    }
}