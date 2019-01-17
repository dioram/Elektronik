using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UniRx;
using UnityEngine;

namespace Elektronik.Online
{
    public class UDPPackagesReceiver
    {
        private uint m_packageNum = 0;
        SortedList<uint, Package> m_packagesBuffer;
        private UdpClient m_network;
        IPEndPoint endPoint;

        public UDPPackagesReceiver(IPAddress ip, int port)
        {
            m_packagesBuffer = new SortedList<uint, Package>();
            endPoint = new IPEndPoint(ip, port);
            m_network = new UdpClient(endPoint);
        }

        public Package GetPackage()
        {
            int countOfBytes = -1;
            
            Package receivedPackage = null;

            Debug.LogFormat("[PackagesReceiver.GetPackage] Buffer size {0}", m_packagesBuffer.Count);

            Debug.LogFormat("[PackagesReceiver.GetPackage] Buffer check");
            if (m_packagesBuffer.Count > 0 && m_packagesBuffer.First().Key == m_packageNum)
            {
                
                receivedPackage = m_packagesBuffer[m_packageNum];
                Debug.LogFormat("[PackagesReceiver.GetPackage] Buffer removing");
                m_packagesBuffer.Remove(m_packageNum);
                Debug.LogFormat("[PackagesReceiver.GetPackage] Buffer removed");
                Debug.LogFormat("[PackagesReceiver.GetPackage] Buffer checked");
                m_packageNum += 1;
                return receivedPackage;
            }
            
            Debug.LogFormat("[PackagesReceiver.GetPackage] Socket check");
            if (m_network.Available > 8)
            {
                Debug.LogFormat("[PackagesReceiver.GetPackage] Receive");
                byte[] datagram = m_network.Receive(ref endPoint);
                Debug.LogFormat("[PackagesReceiver.GetPackage] Received");
                uint packageNum = BitConverter.ToUInt32(datagram, 0);
                countOfBytes = BitConverter.ToInt32(datagram, sizeof(int));
                if (countOfBytes != datagram.Length - 2 * sizeof(int))
                {
                    Debug.LogWarningFormat("Bad package! Read bytes count = {0}; Actual bytes count {1}", countOfBytes, datagram.Length - sizeof(int));
                }
                else
                {
                    byte[] rawPackage = new byte[countOfBytes];
                    Array.Copy(datagram, sizeof(int) * 2, rawPackage, 0, countOfBytes);
                    Debug.LogFormat("[PackagesReceiver.GetPackage] Parse");
                    receivedPackage = Package.Parse(rawPackage);
                    Debug.LogFormat("[PackagesReceiver.GetPackage] Parsed");
                }

                if (packageNum == m_packageNum)
                {
                    m_packageNum += 1;
                }
                else
                {
                    Debug.LogFormat("[PackagesReceiver.GetPackage] Add to buffer");
                    Debug.LogFormat("[PackagesReceiver.GetPackage] Package Num = {0}, receivedPackage = {1}", packageNum, receivedPackage);
                    Debug.LogFormat("[PackagesReceiver.GetPackage] Already in list = {0}", m_packagesBuffer.ContainsKey(packageNum));

                    m_packagesBuffer.Add(packageNum, receivedPackage);
                    Debug.LogFormat("[PackagesReceiver.GetPackage] Added to buffer");
                    receivedPackage = null;
                }
            }
            Debug.LogFormat("[PackagesReceiver.GetPackage] Socket checked");
            // Wi-Fi pass: 5251166525 :)
            return receivedPackage;
        }

        ~UDPPackagesReceiver()
        {
            m_network.Close();
        }
    }
}
