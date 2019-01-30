using Elektronik.Common;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.SlamEventsCommandPattern;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Online
{
    public class SlamEventsManager : MonoBehaviour
    {
        public Text status;
        public Button clear;
        public Button reconnect;

        TCPPackagesReceiver m_receiver;

        public FastLinesCloud linesCloud;
        public FastPointCloud pointCloud;
        public SlamObservationsGraph observationsGraph;
        public Helmet helmet;

        public int connectionTries = 10;

        private ISlamContainer<SlamPoint> m_pointsContainer;
        private ISlamContainer<SlamLine> m_linesContainer;
        private IPackageCSConverter m_converter;
        private bool m_cancelCoroutine = false;
        private bool m_connecting = false;

        private void OnDestroy()
        {
            m_cancelCoroutine = true;
            if (m_receiver != null)
                m_receiver.Dispose();
        }

        private void Awake()
        {
            m_converter = new Camera2Unity3dPackageConverter(Matrix4x4.Scale(Vector3.one * OnlineModeSettings.Current.Scaling));
            m_receiver = new TCPPackagesReceiver();
            m_pointsContainer = new SlamPointsContainer(pointCloud);
            m_linesContainer = new SlamLinesContainer(linesCloud);
        }

        private void Start()
        {
            clear.onClick.AddListener(Clear);
            reconnect.onClick.AddListener(Reconnect);
            status.color = Color.red;
            status.text = "Not connected...";
            var handler =
                Observable.EveryFixedUpdate()
                .Where(_ => m_receiver.Connected)
                .Select(_ => m_receiver.GetPackage())
                .Where(package => package != null)
                .Do(pkg => m_converter.Convert(ref pkg))
                .Do(pkg => Debug.Log(pkg.Timestamp))
                .Do(pkg => new AddCommand(m_pointsContainer, m_linesContainer, observationsGraph, pkg).Execute())
                .Do(pkg => new UpdateCommand(m_pointsContainer, observationsGraph, helmet, pkg).Execute())
                .Do(pkg => new PostProcessingCommand(m_pointsContainer, m_linesContainer, observationsGraph, helmet, pkg).Execute())
                .Do(_ => m_pointsContainer.Repaint())
                .Do(_ => m_linesContainer.Repaint())
                .Do(_ => observationsGraph.Repaint())
                .Subscribe();
        }


        private void Clear()
        {
            m_pointsContainer.Clear();
            m_linesContainer.Clear();
            observationsGraph.Clear();
            helmet.ResetHelmet();
        }

        private void Reconnect()
        {
            if (m_connecting)
                return;
            Observable
                .FromEvent(action => m_receiver.OnDisconnect += () => action(), action => m_receiver.OnDisconnect -= () => action())
                .ObserveOnMainThread()
                .Do(_ => status.color = Color.red)
                .Do(_ => status.text = "Disconnected!")
                .Subscribe();
            status.color = Color.blue;
            Disconnect();
            StartCoroutine(WaitForConnection(connectionTries));
        }

        private void Disconnect()
        {
            m_receiver.Dispose();
            m_receiver = new TCPPackagesReceiver();
        }

        IEnumerator WaitForConnection(int tries)
        {
            m_connecting = true;
            status.color = Color.blue;
            for (int i = 0; i < tries; ++i)
            {
                status.text = String.Format("New connection try... ({0} from {1})", i + 1, tries);
                yield return null;
                if (m_receiver.Connect(OnlineModeSettings.Current.Address, OnlineModeSettings.Current.Port))
                {
                    status.color = Color.green;
                    status.text = "Connected!";
                    m_connecting = false;
                    yield break;
                }
                yield return null /*new WaitForSeconds(1)*/;
            }
            status.color = Color.red;
            status.text = "Not connected...";
            m_connecting = false;
            yield return null;
        }
    }
}
