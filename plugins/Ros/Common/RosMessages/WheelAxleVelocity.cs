using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;
// ReSharper disable InconsistentNaming

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public class WheelAxleVelocity : Message
    {
        public const string RosMessageName = "control_msgs/WheelAxleVelocity";

        public Header? Header { get; set; }
        public byte axle { get; set; }
        public double left_wheel { get; set; }
        public double right_wheel { get; set; }
    }
}