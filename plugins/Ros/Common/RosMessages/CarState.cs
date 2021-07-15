using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public class CarState : Message
    {
        public const string RosMessageName = "control_msgs/CarState";

        public Header Header { get; set; }
        public bool is_brake { get; set; }
        public ushort brake_press { get; set; }
        public bool is_acceleration { get; set; }
        public double acceleration_press { get; set; }
        public bool button_state { get; set; }
    }
}