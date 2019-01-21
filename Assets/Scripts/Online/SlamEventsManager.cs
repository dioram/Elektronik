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
        public Button clear;
        public Button reconnect;

        TCPPackagesReceiver m_receiver;

        public FastLinesCloud linesCloud;
        public FastPointCloud pointCloud;
        public SlamObservationsGraph observationsGraph;
        public Helmet helmet;

        private ISlamContainer<SlamPoint> m_pointsContainer;
        private ISlamContainer<SlamLine> m_linesContainer;
        private IPackageCSConverter m_converter;
        private bool m_cancelCoroutine = false;

        private void OnDestroy()
        {
            m_cancelCoroutine = true;
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
            
            WaitForConnection(100);
            var handler =
                /*Observable.Start(() => m_receiver.GetPackage())
                .RepeatUntilDestroy(gameObject)
                .ObserveOnMainThread(MainThreadDispatchType.FixedUpdate)*/
                Observable.EveryFixedUpdate()
                .Select(_ => m_receiver.GetPackage())
                .Where(_ => m_receiver.Connected)
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
        }

        private void Reconnect()
        {
            WaitForConnection(100);
        }

        void WaitForConnection(int tries)
        {
            for (int i = 0; i < tries; ++i)
            {
                Debug.Log("New connection try...");
                if (m_receiver.Connect(OnlineModeSettings.Current.Address, OnlineModeSettings.Current.Port))
                {
                    break;
                }

            }
        }
    }
}
