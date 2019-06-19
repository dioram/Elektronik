using System;
using System.Net;

namespace Elektronik.Online
{
    [Serializable]
    public class OnlineModeSettings : IComparable<OnlineModeSettings>
    {
        public enum ConnectionTypes
        {
            UDP,
            TCP,
        }

        public OnlineModeSettings()
        {
            MapInfoAddress = IPAddress.Parse("127.0.0.1");
        }

        public static OnlineModeSettings Current { get; set; }

        public IPAddress MapInfoAddress { get; set; }
        public int MapInfoPort { get; set; }
        public float MapInfoScaling { get; set; }

        public IPAddress VRAddress { get; set; }
        public int VRPort { get; set; }
        public float VRScaling { get; set; }
        public ConnectionTypes VRConnectionType { get; set; }

        public DateTime Time { get; set; }

        public int CompareTo(OnlineModeSettings other)
        {
            if (MapInfoAddress.Equals(other.MapInfoAddress) && MapInfoPort == other.MapInfoPort)
                return 0;
            return -1;
        }
    }
}
