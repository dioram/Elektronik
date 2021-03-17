using System.Linq;
using RosSharp.RosBridgeClient;

namespace Elektronik.Ros.RosMessages
{
    public class MarkerArray : Message
    {
        public const string RosMessageName = "visualization_msgs/MarkerArray";

#pragma warning disable 8618
        public Marker[] Markers { get; set; }
#pragma warning restore 8618

        public bool HasDeleteAll => Markers.Any(m => m.Action == Marker.MarkerAction.DeleteAll);
    }
}