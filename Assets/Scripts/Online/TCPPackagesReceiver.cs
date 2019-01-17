using Elektronik.Common;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Elektronik.Online
{
    public class TCPPackagesReceiver
    {
        private TimeoutTcpClient m_network;
        private NetworkStream m_stream;

        public TCPPackagesReceiver()
        {
            m_network = new TimeoutTcpClient();
        }

        public bool Connect(IPAddress ip, int port, TimeSpan timeout)
        {
            try
            {
                m_network.Connect(ip, port, timeout);
                m_stream = m_network.GetStream();
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        public Package GetPackage()
        {
            if (m_stream == null || !m_network.Connected)
            {
                throw new InvalidOperationException("Call Connect before using this [GetPackage] method");
            }
            int countOfPackageBytes = -1;
            if (m_network.Available > sizeof(int) * 2)
            {
                byte[] buffer = new byte[sizeof(int)];
                m_stream.Seek(sizeof(int), System.IO.SeekOrigin.Current);
                m_stream.Read(buffer, 0, sizeof(int));
                countOfPackageBytes = BitConverter.ToInt32(buffer, 0);
            }
            else
            {
                return null;
            }
            byte[] rawPackage = new byte[countOfPackageBytes];
            m_stream.Read(rawPackage, 0, rawPackage.Length);
            return Package.Parse(rawPackage);
        }

        ~TCPPackagesReceiver()
        {
            m_network.Close();
        }
    }
}
