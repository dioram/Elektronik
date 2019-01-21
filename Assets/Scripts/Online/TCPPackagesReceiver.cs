using Elektronik.Common;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Elektronik.Online
{
    public class TCPPackagesReceiver
    {
        private TcpClient m_network;
        private NetworkStream m_stream;

        public bool Connected { get { return m_network.Connected; } }

        public TCPPackagesReceiver()
        {
            m_network = new TcpClient();
        }

        public void Connect(IPAddress ip, int port)
        {
            try
            {
                m_network.Connect(ip.ToString(), port);
                m_stream = m_network.GetStream();
                Debug.Log("Connected!");
            }
            catch
            {
                Debug.Log("Connection failed");
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
                m_stream.Seek(sizeof(int), SeekOrigin.Current); // skip number of package
                m_stream.Read(buffer, 0, sizeof(int)); // read count of package bytes
                countOfPackageBytes = BitConverter.ToInt32(buffer, 0);
                byte[] rawPackage = new byte[countOfPackageBytes];
                m_stream.Read(rawPackage, 0, rawPackage.Length);
                return Package.Parse(rawPackage);
            }
            return null;
        }

        ~TCPPackagesReceiver()
        {
            m_network.Close();
        }
    }
}
