using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public class Command : Message
    {
        public const string RosMessageName = "control_msgs/Command";

        public Header Header { get; set; }
        public byte type { get; set; }
        public double acc { get; set; }
        public double dec { get; set; }
        public double angle { get; set; }
        public double current_long_ref { get; set; }
        public double current_lat_ref { get; set; }
        public double current_lat_ref_yaw { get; set; }
    }
}