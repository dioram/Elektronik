using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;
// ReSharper disable InconsistentNaming

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public class Gga : Message
    {
        
        public const string RosMessageName = "gps_msgs/Gga";

        public Header? Header { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public byte fix_quality { get; set; }
        public ushort sats_online { get; set; }
        public float hdop { get; set; }
        public double altitude { get; set; }
        public double height_of_geoid { get; set; }
        public float time_from_last_update { get; set; }
    }
}