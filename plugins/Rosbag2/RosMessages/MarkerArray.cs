using System.Linq;
using RosSharp.RosBridgeClient;

namespace Elektronik.Rosbag2.RosMessages
{
    public class MarkerArray : Message
    {
        public const string RosMessageName = "visualization_msgs/MarkerArray";

        public Marker[] Markers { get; set; }

        public bool HasDeleteAll => Markers.Any(m => m.Action == Marker.MarkerAction.DeleteAll);
    }
}