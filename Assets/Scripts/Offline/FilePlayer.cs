using Elektronik.Offline.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Elektronik.Common;

namespace Elektronik.Offline
{
    public class FilePlayer : MonoBehaviour
    {
        EventFilePlayer m_eventFilePlayer;
        PointCloudPainter m_cloudPainter;

        private void Awake()
        {
            
        }

        void Start()
        {
            Debug.Log("Analyzing");
            ISlamEvent[] events = EventReader.AnalyzeFile(FileModeSettings.Path);
            Debug.Log("Analyzed");
            m_eventFilePlayer = new EventFilePlayer(events);
            m_cloudPainter = GameObject.Find("Point cloud painter").GetComponent<PointCloudPainter>();
        }

        private void FixedUpdate()
        {
            if (m_eventFilePlayer != null)
            {
                m_cloudPainter.Clear();
                GState state = m_eventFilePlayer.NextEvent();
                SlamPoint[] points = state.PointCloud.Select(pc => pc.Value).Where(sp => !sp.IsRemoved).ToArray();
                Vector3[] pointsPositions = points.Select(sp => sp.Position).ToArray();
                Color[] pointsColors = points.Select(sp => sp.Color).ToArray();
                m_cloudPainter.DrawCloud(pointsPositions, pointsColors);
                Debug.Log(state.CurrentEvent.EventType.ToString());
            }
        }
    }
}