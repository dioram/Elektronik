using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using VRTK;

namespace Elektronik.Online
{
    [RequireComponent(typeof(VRHackerUDPConnection))]
    public class VRHacker : MonoBehaviour
    {
        private VRHackerUDPConnection m_connection;
        private Pose m_lastPose;
        private Thread m_dataListener;
        private bool m_stop;

        // Start is called before the first frame update
        void Awake()
        {
            m_connection = GetComponent<VRHackerUDPConnection>();
            m_dataListener = new Thread(Listen);
        }

        void OnEnable()
        {
            m_lastPose = new Pose();
            m_lastPose.rotation = Quaternion.identity;
            m_lastPose.position = Vector3.zero;
            m_stop = false;
            m_dataListener.Start();
        }

        void OnDisable()
        {
            m_stop = true;
            m_dataListener.Join();
        }

        void Listen()
        {
            while (!m_stop)
            {
                Pose newPose;
                if (m_connection.GetPose(out newPose))
                {
                    m_lastPose = newPose;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            HackPose();
        }

        private void HackPose()
        {
            Transform headset = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
            if (headset != null)
            {
                headset.rotation = m_lastPose.rotation;
                headset.position = m_lastPose.position;
            }
        }
    }
}
