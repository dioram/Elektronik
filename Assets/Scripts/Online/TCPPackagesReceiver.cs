using Elektronik.Common;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Elektronik.Online
{
    public class TCPPackagesReceiver : IDisposable
    {
        Thread m_readingThread;
        object m_sync;
        List<Package> m_packagesBuffer;
        Queue<Package> m_readPackages;
        private int m_packageNum = 0;
        private TcpClient m_network;
        private NetworkStream m_stream;
        private bool m_stop;
        private IPGlobalProperties m_ipProperties;
        private TcpConnectionInformation[] m_tcpConnections;

        public bool Connected { get { return m_network.Connected; } }

        public TCPPackagesReceiver()
        {
            m_network = new TcpClient();
            m_network.ReceiveBufferSize = 64 * 1024;
            m_packagesBuffer = new List<Package>();
            m_readPackages = new Queue<Package>();
            m_sync = new object();
            m_readingThread = new Thread(ReadPackages);
            m_readingThread.IsBackground = true;
        }

        private void ReadPackages()
        {
            if (m_stream == null || !m_network.Connected)
            {
                throw new InvalidOperationException("Call Connect before using this [GetPackage] method");
            }
            Package receivedPackage = null;
            while (!m_stop)
            {
                while (m_network.Available > 4)
                {
                    byte[] rawCountOfBytes = new byte[sizeof(int)];
                    m_stream.Read(rawCountOfBytes, 0, sizeof(int));
                    int countOfBytes = BitConverter.ToInt32(rawCountOfBytes, 0);
                    byte[] rawPackage = new byte[countOfBytes];
                    while (m_network.Available < countOfBytes)
                    {
                        Debug.LogFormat("available data = {0}", m_network.Available);
                    }
                    m_stream.Read(rawPackage, 0, countOfBytes);
                    receivedPackage = Package.Parse(rawPackage);
                    m_packagesBuffer.Add(receivedPackage);
                    receivedPackage = m_packagesBuffer.Find(pkg => pkg.Timestamp == m_packageNum);
                    if (receivedPackage != null)
                    {
                        lock (m_sync)
                        {
                            m_readPackages.Enqueue(receivedPackage);
                        }
                        m_packagesBuffer.Remove(receivedPackage);
                        ++m_packageNum;
                    }
                }
            }
            Debug.Log("Disconnected or stopped");
        }

        public bool Connect(IPAddress ip, int port)
        {
            try
            {
                m_network.Connect(ip.ToString(), port);
                m_stream = m_network.GetStream();
                m_stop = false;
                m_readingThread.Start();
                Debug.Log("Connected!");
                return true;
            }
            catch
            {
                Debug.Log("Connection failed");
            }
            return false;
        }

        public Package GetPackage()
        {
            Package package = null;
            lock (m_sync)
            {
                if (m_readPackages.Count > 0)
                {
                    package = m_readPackages.Dequeue();
                }
            }
            return package;
        }

        public void Dispose()
        {
            m_stop = true;
            m_network.Close();
            Debug.Log("[TCPPackagesReceiver] disposing");
            if (m_readingThread.ThreadState == ThreadState.Running)
                m_readingThread.Join();
            Debug.Log("[TCPPackagesReceiver] disposed");
        }
    }
}
