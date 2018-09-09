using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Elektronik.Common
{
    public class PointCloudPainter : MonoBehaviour
    {
        public GameObject meshPrefab;
        public Material matVertex;
        public float scale = 1;

        private const int LIMIT_POINTS = 65000;

        private Vector3[] m_points;
        private Color[] m_colors;
        private Vector3 m_minValue;


        public void UpdatePoints(Vector3[] points, Color[] colors)
        {
            ClearMeshes();
            m_points = points;
            m_colors = colors;
            // Instantiate Point Groups
            int numPointGroups = Mathf.CeilToInt(m_points.Length * 1.0f / LIMIT_POINTS * 1.0f);

            for (int i = 0; i < numPointGroups - 1; i++)
            {
                InstantiateMesh(i, LIMIT_POINTS);
            }
            InstantiateMesh(numPointGroups - 1, m_points.Length - (numPointGroups - 1) * LIMIT_POINTS);
        }


        void InstantiateMesh(int meshInd, int nPoints)
        {
            // Create Mesh
            GameObject pointGroup = MF_AutoPool.Spawn(meshPrefab);
            pointGroup.name = meshInd.ToString();
            pointGroup.GetComponent<Renderer>().material = matVertex;

            Mesh mesh = pointGroup.GetComponent<MeshFilter>().mesh;
            if (mesh != null)
            {
                mesh.Clear();
            }
            else
            {
                mesh = new Mesh();
            }
            FillMesh(meshInd, nPoints, LIMIT_POINTS, ref mesh);
            pointGroup.GetComponent<MeshFilter>().mesh = mesh;
            
        }

        void FillMesh(int id, int nPoints, int limitPoints, ref Mesh mesh)
        {
            mesh.Clear();
            Vector3[] myPoints = new Vector3[nPoints];
            int[] indices = new int[nPoints];
            Color[] myColors = new Color[nPoints];

            for (int i = 0; i < nPoints; ++i)
            {
                myPoints[i] = (m_points[id * limitPoints + i] - m_minValue) * scale;
                indices[i] = i;
                myColors[i] = m_colors[id * limitPoints + i];
            }

            mesh.vertices = myPoints;
            mesh.colors = myColors;
            mesh.SetIndices(indices, MeshTopology.Points, 0);

            mesh.RecalculateBounds();
        }

        void calculateMin(Vector3 point)
        {
            if (m_minValue.magnitude == 0)
                m_minValue = point;

            if (point.x < m_minValue.x)
                m_minValue.x = point.x;
            if (point.y < m_minValue.y)
                m_minValue.y = point.y;
            if (point.z < m_minValue.z)
                m_minValue.z = point.z;
        }

        void ClearMeshes()
        {
            MF_AutoPool.DespawnPool(meshPrefab);
        }

    }
}