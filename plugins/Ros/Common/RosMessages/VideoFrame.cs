using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public class VideoFrame : Message
    {
        public const string RosMessageName = "sensing_msgs/VideoFrame";

        public Header Header { get; set; }
        public ulong dts { get; set; }
        public ulong pts { get; set; }
        public ulong Duration { get; set; }
        public byte[] Data { get; set; }
    }
}