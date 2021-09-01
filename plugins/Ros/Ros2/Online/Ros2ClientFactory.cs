using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2ClientFactory : ElektronikPluginsFactoryBase<Ros2Settings>, IDataSourcePluginsOnlineFactory
    {
        public override IElektronikPlugin Start(ICSConverter converter) => new Ros2Client(TypedSettings);
        public override string DisplayName => "ROS2 listener";
        public override string Description => "Client for " +
                "<#7f7fe5><u><link=\"https://docs.ros.org/en/foxy/index.html\">ROS2</link></u></color> network.";
        public override string Version => "1.3";
    }
}