using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

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
            Address = IPAddress.Parse("127.0.0.1");
        }

        public static OnlineModeSettings Current { get; set; }

        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public float Scaling { get; set; }
        public ConnectionTypes ConnectionType { get; set; }
        public DateTime Time { get; set; }

        public int CompareTo(OnlineModeSettings other)
        {
            if (Address.Equals(other.Address) && Port == other.Port)
                return 0;
            return -1;
        }
    }
}
