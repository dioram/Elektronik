using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.Settings.Bags;

namespace Elektronik.RosPlugin.Ros.Online
{
    public class Ros1ClientFactory: ElektronikPluginsFactoryBase<Ros1Settings>, IDataSourcePluginsFactory
    {
        protected override IElektronikPlugin StartPlugin(Ros1Settings settings, ICSConverter converter)
        {
            return new Ros1Client(DisplayName, Logo, settings);
        }

        public override string DisplayName => "ROS listener";
        public override string Description => "Client for " +
                "<#7f7fe5><u><link=\"http://wiki.ros.org/noetic/Installation\">ROS1</link></u></color>" +
                " network. Needs to be used with " +
                "<#7f7fe5><u><link=\"http://wiki.ros.org/rosbridge_suite/Tutorials/RunningRosbridge\">" +
                "Rosbridge</link></u></color>.";
    }
}