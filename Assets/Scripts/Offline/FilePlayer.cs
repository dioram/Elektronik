using Elektronik.Offline.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Elektronik.Common;
using UnityEngine.UI;
using System;

namespace Elektronik.Offline
{
    public class FilePlayer : MonoBehaviour
    {
        EventFilePlayer m_eventFilePlayer;
        PointCloudPainter m_cloudPainter;
        Slider m_timeline;
        Text m_timeLabel;
        public GameObject listViewItemPrefab;
        private Transform m_listViewContent;

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
            m_timeline = GameObject.Find("Time").GetComponent<Slider>();
            m_timeline.maxValue = m_eventFilePlayer.Length;
            m_timeline.minValue = 0;

            m_timeLabel = GameObject.Find("Time label").GetComponent<Text>();
            m_timeLabel.text = "";

            m_listViewContent = GameObject.Find("Events logger").GetComponentInChildren<ScrollRect>().content;
        }

        private void FixedUpdate()
        {
            if (m_eventFilePlayer == null || m_eventFilePlayer.EndOfFile)
                return;

            m_timeline.value = m_eventFilePlayer.Position;

            string time = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", 
                m_eventFilePlayer.CurrentTimestamp.Hours,
                m_eventFilePlayer.CurrentTimestamp.Minutes,
                m_eventFilePlayer.CurrentTimestamp.Seconds,
                m_eventFilePlayer.CurrentTimestamp.Milliseconds);
            m_timeLabel.text = time;
            UpdatePoints();
        }

        private void UpdatePoints()
        {
            GState gState = null;
            GameObject listViewItem = Instantiate(listViewItemPrefab);
            Text listViewItemText = listViewItem.transform.GetComponentInChildren<Text>();
            listViewItemText.fontSize = 14;
            listViewItemText.text = gState.CurrentEvent.ToString();
            if (gState.CurrentEvent.IsKeyEvent)
            {
                listViewItem.GetComponent<Image>().color = Color.green;
            }
            listViewItem.transform.SetParent(m_listViewContent);
            
            

            SlamPoint[] pointsToPaint = gState.PointCloud.Select(kv => kv.Value).Where(v => !v.IsRemoved).ToArray();
            Vector3[] points = pointsToPaint.Select(p => p.Position).ToArray();
            Color[] colors = pointsToPaint.Select(p => p.Color).ToArray();
            m_cloudPainter.UpdatePoints(points, colors);
        }

            //private void UpdatePoints()
            //{
            //    GState gState = m_eventFilePlayer.NextEvent();
            //    SlamPoint[] pointsToPaint = gState.PointCloud.Select(kv => kv.Value).Where(v => !v.IsRemoved).ToArray();
            //    points = pointsToPaint.Select(p => p.Position).ToArray();
            //    colors = pointsToPaint.Select(p => p.Color).ToArray();

            //    // Instantiate Point Groups
            //    numPointGroups = Mathf.CeilToInt(points.Length * 1.0f / limitPoints * 1.0f);

            //    for (int i = 0; i < numPointGroups - 1; i++)
            //    {
            //        InstantiateMesh(i, limitPoints);
            //    }
            //    InstantiateMesh(numPointGroups - 1, points.Length - (numPointGroups - 1) * limitPoints);
            //}


            //void InstantiateMesh(int meshInd, int nPoints)
            //{
            //    // Create Mesh
            //    GameObject pointGroup = new GameObject(meshInd.ToString());
            //    pointGroup.AddComponent<MeshFilter>();
            //    pointGroup.AddComponent<MeshRenderer>();
            //    pointGroup.GetComponent<Renderer>().material = matVertex;

            //    pointGroup.GetComponent<MeshFilter>().mesh = CreateMesh(meshInd, nPoints, limitPoints);
            //    pointGroup.transform.parent = pointCloud.transform;
            //}

            //Mesh CreateMesh(int id, int nPoints, int limitPoints)
            //{
            //    Mesh mesh = new Mesh();

            //    Vector3[] myPoints = new Vector3[nPoints];
            //    int[] indecies = new int[nPoints];
            //    Color[] myColors = new Color[nPoints];

            //    for (int i = 0; i < nPoints; ++i)
            //    {
            //        myPoints[i] = points[id * limitPoints + i] - minValue;
            //        indecies[i] = i;
            //        myColors[i] = colors[id * limitPoints + i];
            //    }

            //    mesh.vertices = myPoints;
            //    mesh.colors = myColors;
            //    mesh.SetIndices(indecies, MeshTopology.Points, 0);
            //    mesh.uv = new Vector2[nPoints];
            //    mesh.normals = new Vector3[nPoints];

            //    return mesh;
            //}

            //void calculateMin(Vector3 point)
            //{
            //    if (minValue.magnitude == 0)
            //        minValue = point;

            //    if (point.x < minValue.x)
            //        minValue.x = point.x;
            //    if (point.y < minValue.y)
            //        minValue.y = point.y;
            //    if (point.z < minValue.z)
            //        minValue.z = point.z;
            //}
        }
}