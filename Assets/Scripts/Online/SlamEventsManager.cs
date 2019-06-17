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
        
        //public GameObject observationPrefab;
        //public FastLinesCloud observationsLinesCloud;
        //public FastLinesCloud linesCloud;
        //public FastPointCloud pointCloud;
        //public Helmet helmet;
        public int connectionTries = 10;
        public RepaintablePackagePresenter[] presenters;
        
        private IDisposable m_mapUpdate;
        private IDisposable m_mapRepaint;
        private bool m_connecting = false;
        private TCPPackagesReceiver m_receiver;
        //private Queue<Pose> m_positions;
        private DataParser m_parser;
        private PackagePresenter m_presenter;

        private void Awake()
        {
            ICSConverter converter = new Camera2Unity3dPackageConverter(Matrix4x4.Scale(Vector3.one * OnlineModeSettings.Current.MapInfoScaling));
            m_parser = new SlamPackageParser(converter);
            m_receiver = new TCPPackagesReceiver(m_parser);
        }

        private void Start()
        {
            //m_positions = new Queue<Pose>();
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
                .Do(m_presenter.Present)
                .Subscribe();

            m_mapRepaint = Observable.EveryFixedUpdate().Do(_ => Repaint())/*.Do(_ => UpdateHelmet())*/.Subscribe();
        }

        private IPackage SafeReadPackage()
        {
            IPackage package = null;
            lock (m_receiver)
            {
                if (m_receiver.Connected)
                    package = m_receiver.GetPackage();
            }
            return package;
        }

        //private void AddPose(IPackage pkg)
        //{
        //    int helmetPoseId = pkg.Observations.FindIndex(obs => obs.Point.id == -1);
        //    if (helmetPoseId != -1)
        //    {
        //        SlamObservation helmet = pkg.Observations[helmetPoseId];
        //        lock (m_positions)
        //        {
        //            m_positions.Enqueue(new Pose(helmet.Point.position, helmet.Orientation));
        //        }
        //    }
        //}
        //private void UpdateHelmet()
        //{
        //    lock (m_positions)
        //    {
        //        if (m_positions.Count > 0)
        //        {
        //            Pose pose = m_positions.Dequeue();
        //            helmet.ReplaceAbs(pose.position, pose.rotation);
        //        }
        //    }
        //}
        private void Repaint()
        {
            foreach (var presenter in presenters)
                presenter.Repaint();
        }
        private void Clear()
        {
            Disconnect();
            foreach (var presenter in presenters)
                presenter.Clear();
            //helmet.ResetHelmet();
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
                m_receiver = new TCPPackagesReceiver(m_parser);
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
