using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;
// ReSharper disable InconsistentNaming

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public class GSensorAcc : Message
    {
        public const string RosMessageName = "control_msgs/GSensorAcc";

        public Header? Header { get; set; }

        public bool valid { get; set; }
        public double lat_accel { get; set; }
        public double long_accel { get; set; }
        public double yaw_rate { get; set; }
        public double speed { get; set; }
    }
}