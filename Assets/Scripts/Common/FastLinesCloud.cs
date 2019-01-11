using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Elektronik.Common
{
    public class FastLinesCloud : MonoBehaviour
    {
        public float scale = 1;

        public LinesMeshObject meshObjectPrefab;
        private Dictionary<int, LinesMeshObject> m_meshObjects;

        private void Awake()
        {
            m_meshObjects = new Dictionary<int, LinesMeshObject>();
        }

        private void CheckMesh(int srcLineIdx, out int meshIdx, out int lineIdx)
        {
            meshIdx = srcLineIdx / LinesMeshObject.MAX_LINES_COUNT;
            if (!m_meshObjects.ContainsKey(meshIdx))
            {
                AddNewMesh(meshIdx);
            }
            lineIdx = srcLineIdx % LinesMeshObject.MAX_LINES_COUNT;
        }

        public bool LineExists(int lineIdx)
        {
            int meshIdx = lineIdx / LinesMeshObject.MAX_LINES_COUNT;
            if (m_meshObjects.ContainsKey(meshIdx))
            {
                return m_meshObjects[meshIdx].LineExists(lineIdx % LinesMeshObject.MAX_LINES_COUNT);
            }
            else
            {
                return false;
            }
        }

        public void GetLine(int idx, out Vector3 position1, out Vector3 position2, out Color color)
        {
            int meshId;
            int lineId;
            CheckMesh(idx, out meshId, out lineId);
            m_meshObjects[meshId].GetLine(lineId, out position1, out position2, out color);
        }

        public void SetLine(int idx, Vector3 position1, Vector3 position2, Color color)
        {
            int meshId;
            int lineId;
            CheckMesh(idx, out meshId, out lineId);
            position1.Scale(scale * Vector3.one);
            position2.Scale(scale * Vector3.one);
            m_meshObjects[meshId].SetLine(lineId, position1, position2, color);
        }

        public void SetLineColor(int idx, Color color)
        {
            int meshId;
            int lineId;
            CheckMesh(idx, out meshId, out lineId);
            m_meshObjects[meshId].SetLineColor(lineId, color);
        }

        public void SetLinePosition(int idx, Vector3 position1, Vector3 position2)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            position1.Scale(scale * Vector3.one);
            position2.Scale(scale * Vector3.one);
            m_meshObjects[meshId].SetLinePositions(pointId, position1, position2);
        }

        public void SetLines(int[] idxs, Vector3[] positions1, Vector3[] positions2, Color[] colors)
        {
            Debug.Assert((idxs.Length == positions1.Length) && (positions1.Length == positions2.Length) && (positions2.Length == colors.Length));
            for (int i = 0; i < idxs.Length; ++i)
            {
                SetLine(idxs[i], positions1[i], positions2[i], colors[i]);
            }
        }

        public void Clear()
        {
            foreach (var meshObject in m_meshObjects)
            {
                meshObject.Value.Clear();
                MF_AutoPool.Despawn(meshObject.Value.gameObject);
            }
            m_meshObjects.Clear();
        }

        public void Repaint()
        {
            foreach (var meshObject in m_meshObjects)
            {
                meshObject.Value.Repaint();
            }
        }


        private void AddNewMesh(int idx)
        {
            LinesMeshObject newMesh = MF_AutoPool.Spawn(meshObjectPrefab.gameObject).GetComponent<LinesMeshObject>();
            m_meshObjects.Add(idx, newMesh);
        }
    }
}
