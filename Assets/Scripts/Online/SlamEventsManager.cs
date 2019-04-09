using Elektronik.Common;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.SlamEventsCommandPattern;
using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public GameObject observationPrefab;
        public FastLinesCloud observationsLinesCloud;
        public FastLinesCloud linesCloud;
        public FastPointCloud pointCloud;
        public Helmet helmet;
        public int connectionTries = 10;

        private IDisposable m_mapUpdate;
        private ISlamContainer<SlamObservation> m_observationsContainer;
        private ISlamContainer<SlamPoint> m_pointsContainer;
        private ISlamContainer<SlamLine> m_linesContainer;
        private IPackageCSConverter m_converter;
        private bool m_connecting = false;
        private TCPPackagesReceiver m_receiver;

        private void SubscribeToIncome()
        {
            m_mapUpdate = Observable.EveryFixedUpdate()
                .Where(_ => m_receiver.Connected)
                .Select(_ => m_receiver.GetPackage())
                .Where(package => package != null)
                .Do(pkg => m_converter.Convert(ref pkg))
                .Do(UpdateMaps)
                .Do(RepaintMaps)
                .Do(PostProcessMaps)
                .Do(UpdateHelmet)
                .Subscribe();
        }

        private void UpdateHelmet(Package pkg)
        {
            int helmetObsId = pkg.Observations.FindIndex(o => o.Point.id == -1);
            if (helmetObsId != -1)
            {
                SlamObservation obs = pkg.Observations[helmetObsId];
                helmet.ReplaceAbs(obs.Point.position, obs.Orientation);
            }
        }

        private void RepaintMaps(Package pkg)
        {
            m_pointsContainer.Repaint();
            m_linesContainer.Repaint();
            m_observationsContainer.Repaint();
        }

        private void UpdateMaps(Package pkg)
        {
            UpdateMap(pkg.Points, p => p.isNew, p => p.isRemoved, p => p.justColored, p => p.id != -1, m_pointsContainer);
            UpdateMap(
                pkg.Lines, 
                /*isNew*/ _ => true, /*isRemoved*/ _ => false, /*justColored*/ _ => false, /*isValid*/ _ => true, 
                m_linesContainer);
            UpdateMap(
                pkg.Observations, 
                o => o.Point.isNew, o => o.Point.isRemoved, o => o.Point.justColored, o => o.Point.id != -1, 
                m_observationsContainer);
        }

        private void PostProcessMaps(Package pkg)
        {
            if (pkg.Points != null)
            {
                SlamPoint[] updatedPoints = pkg.Points
                    .Where(p => p.id != -1)
                    .Where(p => !p.isRemoved)
                    .Select(p => { var mp = m_pointsContainer.Get(p); mp.color = mp.defaultColor; return mp; })
                    .ToArray();
                UpdateMap(
                    updatedPoints, 
                    /*isNew*/ _ => false, /*isRemoved*/ _ => false, /*justColored*/ _ => true, p => p.id != -1, 
                    m_pointsContainer);
            }
            if (pkg.Lines != null)
            {
                UpdateMap(
                    pkg.Lines, 
                    /*isNew*/ _ => false, /*isRemoved*/ _ => true, /*justColored*/ _ => false, /*isValid*/_ => true, 
                    m_linesContainer);
            }
        }

        private void UpdateMap<T>(
            ICollection<T> source,
            Func<T, bool> isNewSelector,
            Func<T, bool> isRemovedSelector,
            Func<T, bool> justColoredSelector,
            Func<T, bool> isValidSelector,
            ISlamContainer<T> map)
        {
            if (source != null)
            {
                foreach (var element in source)
                {
                    if (isValidSelector(element))
                    {
                        if (isNewSelector(element))
                            map.Add(element);
                        else if (isRemovedSelector(element))
                            map.Remove(element);
                        else if (justColoredSelector(element))
                            map.ChangeColor(element);
                        else
                            map.Update(element);
                    }
                }

            }
        }

        private void Awake()
        {
            m_converter = new Camera2Unity3dPackageConverter(Matrix4x4.Scale(Vector3.one * OnlineModeSettings.Current.MapInfoScaling));
            m_receiver = new TCPPackagesReceiver();

            m_pointsContainer = new SlamPointsContainer(pointCloud);
            m_linesContainer = new SlamLinesContainer(linesCloud);
            m_observationsContainer = new SlamObservationsContainer(observationPrefab, new SlamLinesContainer(observationsLinesCloud));
        }

        private void Start()
        {
            clear.onClick.AddListener(Clear);
            reconnect.onClick.AddListener(Reconnect);
            status.color = Color.red;
            status.text = "Not connected...";
        }

        private void OnDestroy()
        {
            if (m_receiver != null)
                m_receiver.Dispose();
            if (m_mapUpdate != null)
                m_mapUpdate.Dispose();
        }

        private void Clear()
        {
            if (m_mapUpdate != null)
            {
                m_mapUpdate.Dispose();
                m_mapUpdate = null;
            }
            m_pointsContainer.Clear();
            m_linesContainer.Clear();
            m_observationsContainer.Clear();
            helmet.ResetHelmet();
        }

        private void Reconnect()
        {
            if (m_connecting)
                return;
            m_connecting = true;
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
            status.color = Color.blue;
            for (int i = 0; i < tries; ++i)
            {
                status.text = String.Format("New connection try... ({0} from {1})", i + 1, tries);
                yield return null;
                if (m_receiver.Connect(OnlineModeSettings.Current.MapInfoAddress, OnlineModeSettings.Current.MapInfoPort))
                {
                    status.color = Color.green;
                    status.text = "Connected!";
                    Observable
                        .FromEvent(action => m_receiver.OnDisconnect += () => action(), action => m_receiver.OnDisconnect -= () => action())
                        .ObserveOnMainThread()
                        .Do(_ => status.color = Color.red)
                        .Do(_ => status.text = "Disconnected!")
                        .Subscribe();
                    SubscribeToIncome();
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
