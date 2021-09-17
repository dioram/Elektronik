using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;

namespace Elektronik.RosPlugin.Ros2.Bag
{
    public class Ros2ReaderFactory: ElektronikPluginsFactoryBase<Rosbag2Settings>, IDataSourcePluginsFactory
    {
        protected override IElektronikPlugin StartPlugin(Rosbag2Settings settings, ICSConverter converter)
        {
            return new Rosbag2Reader(DisplayName, Logo, settings);
        }

        public override string DisplayName => "ROS2 bag";
        public override string Description => "This plugins allows Elektronik to read data saved from " +
                "<#7f7fe5><u><link=\"https://docs.ros.org/en/foxy/index.html\">ROS2</link></u></color> using " +
                "<#7f7fe5><u><link=\"https://docs.ros.org/en/foxy/Tutorials/Ros2bag/Recording-And-Playing-Back-Data.html\">" +
                "rosbag2</link></u></color>.";
    }
}