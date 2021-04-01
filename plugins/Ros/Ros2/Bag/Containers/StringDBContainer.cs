using Elektronik.Renderers;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class StringDBContainer : DBContainerToWindow<String, StringRenderer, string>
    {
        public StringDBContainer(string displayName, SQLiteConnection dbModel, Topic topic, long[] actualTimestamps) 
                : base(displayName, dbModel, topic, actualTimestamps)
        {
        }

        protected override string ToRenderType(String message)
        {
            return message.data;
        }
    }
}