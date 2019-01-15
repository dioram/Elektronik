using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Elektronik.Online
{
    [Serializable]
    public class OnlineSettings : IComparable<OnlineSettings>
    {
        public enum ConnectionTypes
        {
            UDP,
            TCP,
        }

        public static OnlineSettings Current { get; set; }

        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public float Scaling { get; set; }
        public ConnectionTypes ConnectionType { get; set; }

        public int CompareTo(OnlineSettings other)
        {
            return -1;
        }
    }
}
