using System.Threading;
using UnityEngine;
using VRTK;

namespace Elektronik.Online
{
    public class VRHackerDirect : MonoBehaviour
    {
        private Common.Interop.OMW.Tracker m_tracker;

        private Pose m_lastPose;
        private Thread m_dataListener;
        private bool m_stop;

        void Start()
        {
            Debug.Log("Start creating tracker");
            m_tracker = new Common.Interop.OMW.Tracker();
            Debug.Log("Finished creating tracker. Started tracker initializing");
            m_tracker.Init();
            Debug.Log("Finished tracker initializing");
            m_dataListener = new Thread(Listen);
            m_dataListener.Start();
        }

        void OnEnable()
        {
            m_lastPose = new Pose();
            m_lastPose.rotation = Quaternion.identity;
            m_lastPose.position = Vector3.zero;
            m_stop = false;
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
                if (m_tracker.GetAbsPose(out newPose))
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
                Quaternion quaternion = m_lastPose.rotation;
                quaternion.x *= -1f; quaternion.y *= -1f;
                headset.rotation = quaternion;
                Vector3 position = m_lastPose.position;
                position.x *= -1f; position.y *= -1f;
                headset.position = position;
            }
        }
    }
}
