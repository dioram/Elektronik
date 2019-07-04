using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Parsers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Elektronik.Online.Receivers
{
    public class TCPPackagesReceiver : IDisposable
    {
        Thread m_readingThread;
        List<IPackage> m_packagesBuffer;
        Queue<IPackage> m_readPackages;
        private int m_packageNum = 0;
        private TcpClient m_network;
        private NetworkStream m_stream;
        private bool m_stop;
        private IPGlobalProperties m_ipProperties;
        private TcpConnectionInformation[] m_tcpConnections;
        private DataParser m_parser;

        public delegate void TCPPackagesReceiverHandler();
        public event TCPPackagesReceiverHandler OnDisconnect;

        public bool Connected { get { return m_network.Connected; } }

        public TCPPackagesReceiver(DataParser parser)
        {
            m_network = new TcpClient();
            m_network.ReceiveBufferSize = 64 * 1024;
            m_packagesBuffer = new List<IPackage>(20);
            m_readPackages = new Queue<IPackage>(100);
            m_readingThread = new Thread(ReadPackages);
            m_readingThread.IsBackground = true;
            m_parser = parser;
        }

        private bool CheckConnection(byte[] zeroByte)
        {
            bool isConnected = false;
            if (m_network.Client.Poll(-1, SelectMode.SelectRead))
            {
                byte[] buff = new byte[1];
                try
                {
                    if (m_network.Client.Receive(buff, SocketFlags.Peek) != 0)
                    {
                        isConnected = true;
                    }
                }
                catch (SocketException e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
            return isConnected;
        }

        private void ReadPackages()
        {
            if (m_stream == null || !m_network.Connected)
            {
                throw new InvalidOperationException("Call Connect before using this [GetPackage] method");
            }
            byte[] zeroByte = new byte[1];
            IPackage receivedPackage = null;
            while (!m_stop)
            {
                while (m_network.Available > 4)
                {
                    byte[] rawCountOfBytes = new byte[sizeof(int)];
                    m_stream.Read(rawCountOfBytes, 0, sizeof(int));
                    int countOfBytes = BitConverter.ToInt32(rawCountOfBytes, 0);
                    byte[] rawPackage = new byte[countOfBytes];
                    int readBytes = 0;
                    do
                    {
                        readBytes += m_stream.Read(rawPackage, readBytes, countOfBytes - readBytes);
                        if (m_stop)
                            return;
                    } while (readBytes != countOfBytes);
                    m_parser.Parse(rawPackage, 0, out receivedPackage);
                    if (receivedPackage.Timestamp != m_packageNum)
                    {
                        m_packagesBuffer.Add(receivedPackage);
                        receivedPackage = m_packagesBuffer.Find(pkg => pkg.Timestamp == m_packageNum);
                    }
                    if (receivedPackage != null)
                    {
                        lock (m_readPackages) m_readPackages.Enqueue(receivedPackage);
                        m_packagesBuffer.Remove(receivedPackage);
                        ++m_packageNum;
                    }
                }
                if (!CheckConnection(zeroByte))
                {
                    m_stop = true;
                }
            }
            Debug.Log("Disconnected or stopped");
            OnDisconnect?.Invoke();
        }

        public bool Connect(IPAddress ip, int port)
        {
            try
            {
                m_network.Connect(ip.ToString(), port);
            }
            catch
            {
                Debug.Log("Connection failed");
                return false;
            }
            m_stream = m_network.GetStream();
            m_stop = false;
            m_readingThread.Start();
            Debug.Log("Connected!");
            return true;
        }

        public IPackage GetPackage()
        {
            IPackage package = null;
            lock (m_readPackages)
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
            Debug.Log("[TCPPackagesReceiver] disposing");
            m_stop = true;
            if (m_readingThread.IsAlive)
                m_readingThread.Join();
            m_network.Close();
            Debug.Log("[TCPPackagesReceiver] disposed");
        }
    }
}
