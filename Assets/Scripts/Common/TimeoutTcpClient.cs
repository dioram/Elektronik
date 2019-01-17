using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Elektronik.Common
{
    public class TimeoutTcpClient : TcpClient
    {
        public void Connect(IPAddress ip, int port, TimeSpan timeout)
        {
            var result = BeginConnect(ip, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(timeout);
            if (!success)
            {
                throw new TimeoutException("Failed to connect.");
            }
            EndConnect(result);
        }
    }
}
