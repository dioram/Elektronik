using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;
using Message = RosSharp.RosBridgeClient.Message;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class UnknownTypeDBContainer : DBContainerToWindow<Message, StringRenderer, string>
    {
        public UnknownTypeDBContainer(string displayName, SQLiteConnection dbModel, Topic topic, long[] actualTimestamps) 
                : base(displayName, dbModel, topic, actualTimestamps)
        {
        }

        protected override string ToRenderType(Message message)
        {
            return message.GetData();
        }
    }
}