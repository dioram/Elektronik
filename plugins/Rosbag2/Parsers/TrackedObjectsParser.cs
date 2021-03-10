using Elektronik.Commands;
using Elektronik.Commands.Generic;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Offline;
using Elektronik.Rosbag2.Data;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.Rosbag2.Parsers
{
    public class TrackedObjectsParser : DataParser<(Message message, Topic topic)>
    {
        private readonly ITrackedContainer<SlamTrackedObject> _container;
        private readonly Topic _topic;

        public TrackedObjectsParser(ITrackedContainer<SlamTrackedObject> container, Topic topic)
        {
            _container = container;
            _topic = topic;
        }

        public override ICommand GetCommand((Message message, Topic topic) data)
        {
            if (!data.topic.Equals(_topic)) return Successor?.GetCommand(data);
            
            var message = MessageParser.Parse(data.message, data.topic);
            if (Rosbag2ContainerTree.SupportedMessages.ContainsKey(data.topic.Type)
                && Rosbag2ContainerTree.SupportedMessages[data.topic.Type] == typeof(TrackedObjectsContainer))
            {
                var objs = new[] { message.ToTrackedObject(Converter) };
                
                if (_container.Count == 0)
                {
                    return new AddCommand<SlamTrackedObject>(_container, objs);
                }

                return new UpdateCommand<SlamTrackedObject>(_container, objs);
            }
            
            return Successor?.GetCommand(data);
        }
    }
}