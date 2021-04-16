using System;

namespace Elektronik.RosPlugin.Ros2.Online.Handlers
{
    public class TopicsHandler : TopicsDiscoveryHandler
    {
        public event Action<string, string>? OnNewTopic; 
        
        public override void Handle(string name, string type)
        {
             OnNewTopic?.Invoke(MessageExt.GetRosTopic(name), MessageExt.GetRosType(type));
        }
    }
}