using Elektronik.PluginsSystem;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagReaderFactory: ElektronikPluginsFactoryBase<RosbagSettings>, IDataSourcePluginsFactory
    {
        protected override IElektronikPlugin StartPlugin(RosbagSettings settings)
        {
            return new RosbagReader(DisplayName, Logo, settings);
        }

        public override string DisplayName => "ROS bag";

        public override string Description => "This plugins allows Elektronik to read data saved from " +
                "<#7f7fe5><u><link=\"https://www.ros.org\">ROS</link></u></color>" +
                " using <#7f7fe5><u><link=\"http://wiki.ros.org/rosbag\">rosbag</link></u></color>.";
    }
}