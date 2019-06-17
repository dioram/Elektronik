using Elektronik.Common;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private IDisposable m_mapRepaint;
        private ICloudObjectsContainer<SlamObservation> m_observationsContainer;
        private ICloudObjectsContainer<SlamPoint> m_pointsContainer;
        private ICloudObjectsContainer<SlamLine> m_linesContainer;
        private IPackageCSConverter m_converter;
        private bool m_connecting = false;
        private TCPPackagesReceiver m_receiver;
        private Queue<Pose> m_positions;

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
            m_positions = new Queue<Pose>();
            clear.OnClickAsObservable().Subscribe(_ => Clear());
            reconnect.OnClickAsObservable().Subscribe(_ => Reconnect());
            status.color = Color.red;
            status.text = "Not connected...";
            
        }

        private void OnDestroy()
        {
            if (m_mapUpdate != null)
                m_mapUpdate.Dispose();
            if (m_mapRepaint != null)
                m_mapRepaint.Dispose();
        }

        private void SubscribeToIncome()
        {
            var pkgSrc = new Subject<SlamPackage>();
            m_mapUpdate = Observable.Interval(TimeSpan.FromMilliseconds(0))
                .TakeWhile(_ => m_receiver.Connected)
                .Select(_ => SafeReadPackage())
                .Where(package => package != null)
                .Do(pkg => m_converter.Convert(ref pkg))
                .Do(AddPose)
                .Do(UpdateMaps)
                .Do(PostProcessMaps)
                .Subscribe();

            m_mapRepaint = Observable.EveryFixedUpdate().Do(_ => RepaintMaps()).Do(_ => UpdateHelmet()).Subscribe();
        }

        private SlamPackage SafeReadPackage()
        {
            SlamPackage package = null;
            lock (m_receiver)
            {
                if (m_receiver.Connected)
                    package = m_receiver.GetPackage();
            }
            return package;
        }

        private void AddPose(SlamPackage pkg)
        {
            int helmetPoseId = pkg.Observations.FindIndex(obs => obs.Point.id == -1);
            if (helmetPoseId != -1)
            {
                SlamObservation helmet = pkg.Observations[helmetPoseId];
                lock (m_positions)
                {
                    m_positions.Enqueue(new Pose(helmet.Point.position, helmet.Orientation));
                }
            }
        }
        private void UpdateHelmet()
        {
            lock (m_positions)
            {
                if (m_positions.Count > 0)
                {
                    Pose pose = m_positions.Dequeue();
                    helmet.ReplaceAbs(pose.position, pose.rotation);
                }
            }
            
        }

        private void RepaintMaps()
        {
            lock (m_pointsContainer) m_pointsContainer.Repaint();
            lock (m_linesContainer) m_linesContainer.Repaint();
            lock (m_observationsContainer) m_observationsContainer.Repaint();
        }

        private void UpdateMaps(SlamPackage pkg)
        {
            lock (m_pointsContainer)
                UpdateMap(pkg.Points, p => p.isNew, p => p.isRemoved, p => p.justColored, p => p.id != -1, m_pointsContainer);
            lock (m_linesContainer)
                UpdateMap(
                      pkg.Lines,
                      /*isNew*/ _ => true, /*isRemoved*/ _ => false, /*justColored*/ _ => false, /*isValid*/ _ => true,
                      m_linesContainer);
            lock (m_observationsContainer)
                UpdateMap(
                      pkg.Observations,
                      o => o.Point.isNew, o => o.Point.isRemoved, o => o.Point.justColored, o => o.Point.id != -1,
                      m_observationsContainer);
        }

        private void PostProcessMaps(SlamPackage pkg)
        {
            if (pkg.Points != null)
            {
                SlamPoint[] updatedPoints = pkg.Points
                    .AsParallel()
                    .Where(p => p.id != -1)
                    .Where(p => !p.isRemoved)
                    .Select(p => { var mp = m_pointsContainer[p]; mp.color = mp.defaultColor; return mp; })
                    .ToArray();
                lock (m_pointsContainer)
                    UpdateMap(
                      updatedPoints,
                      /*isNew*/ _ => false, /*isRemoved*/ _ => false, /*justColored*/ _ => true, p => p.id != -1,
                      m_pointsContainer);
            }
            if (pkg.Lines != null)
            {
                lock (m_linesContainer)
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
            ICloudObjectsContainer<T> map)
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

        private void Clear()
        {
            Disconnect();
            lock (m_pointsContainer) m_pointsContainer.Clear();
            lock (m_linesContainer) m_linesContainer.Clear();
            lock (m_observationsContainer) m_observationsContainer.Clear();
            lock (m_positions) m_positions.Clear();
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
            if (m_mapUpdate != null)
                m_mapUpdate.Dispose();
            if (m_mapRepaint != null)
                m_mapRepaint.Dispose();
            lock (m_receiver)
            {
                if (m_receiver != null)
                    m_receiver.Dispose();
                m_receiver = new TCPPackagesReceiver();
            }
        }

        IEnumerator WaitForConnection(int tries)
        {
            status.color = Color.blue;
            for (int i = 0; i < tries; ++i)
            {
                status.text = $"New connection try... ({i + 1} from {tries})";
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
                        .Do(_ => Disconnect())
                        .Subscribe();
                    SubscribeToIncome();
                    m_connecting = false;
                    yield break;
                }
                yield return null;
            }
            status.color = Color.red;
            status.text = "Not connected...";
            m_connecting = false;
            yield return null;
        }
    }
}
