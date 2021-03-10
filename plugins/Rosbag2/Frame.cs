using Elektronik.Commands;
using Elektronik.Offline;
using Elektronik.Rosbag2.Data;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.Rosbag2
{
    public class Frame
    {
        public long Timestamp;
        public Topic Topic;
        public ICommand Command;

        public void Show()
        {
            Command?.Execute();
        }

        public void Rewind()
        {
            Command?.UnExecute();
        }

        public static Frame ParseMessage(long timestamp, Message message, Topic topic, DataParser<(Message, Topic)> parsersChain)
        {
            return new Frame
            {
                Timestamp = timestamp,
                Topic = topic,
                Command = parsersChain.GetCommand((message, topic)),
            };
        }
    }
}