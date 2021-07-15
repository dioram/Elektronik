using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public class WheelAxleVelocity : Message
    {
        public const string RosMessageName = "control_msgs/WheelAxleVelocity";

        public Header Header { get; set; }
        public byte axle { get; set; }
        public double left_wheel { get; set; }
        public double right_wheel { get; set; }
    }
}