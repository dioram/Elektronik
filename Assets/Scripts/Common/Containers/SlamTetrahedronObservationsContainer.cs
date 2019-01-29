using Elektronik.Common.Clouds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamTetrahedronObservationsContainer : ISlamContainer<SlamObservation>
    {
        private SortedDictionary<int, SlamObservation> m_observations;
        private FastTrianglesCloud m_trianglesCloud;

        private int m_added = 0;
        private int m_removed = 0;
        private int m_diff = 0;

        public SlamTetrahedronObservationsContainer(FastTrianglesCloud cloud)
        {
            m_observations = new SortedDictionary<int, SlamObservation>();
            m_trianglesCloud = cloud;
        }

        public int Add(SlamObservation observation)
        {
            ++m_diff;
            ++m_added;
            m_trianglesCloud.SetTetrahedron(
                observation.id,
                Matrix4x4.TRS(observation.position, observation.orientation, Vector3.one),
                observation.color);
            Debug.AssertFormat(!m_observations.ContainsKey(observation.id), "Point with id {0} already in dictionary!", observation.id);
            m_observations.Add(observation.id, observation);
            return observation.id;
        }

        public void AddRange(SlamObservation[] observations)
        {
            foreach (var point in observations)
            {
                Add(point);
            }
        }

        public void Update(SlamObservation observation)
        {
            Debug.AssertFormat(m_observations.ContainsKey(observation.id), "[Update] Container doesn't contain point with id {0}", observation.id);
            SlamObservation current = m_observations[observation.id];
            Matrix4x4 to = Matrix4x4.TRS(observation.position, observation.orientation, Vector3.one);
            current.position = observation.position;
            current.orientation = observation.orientation;
            current.color = observation.color;
            m_trianglesCloud.SetTetrahedron(observation.id, to, observation.color);
            m_observations[observation.id] = current;
        }

        public void ChangeColor(SlamObservation observation)
        {
            //Debug.LogFormat("[Change color] point {0} color: {1}", point.id, point.color);
            Debug.AssertFormat(m_observations.ContainsKey(observation.id), "[Change color] Container doesn't contain point with id {0}", observation.id);
            m_trianglesCloud.SetTetrahedron(observation.id, observation.color);
            SlamObservation current = m_observations[observation.id];
            current.color = observation.color;
            m_observations[observation.id] = current;
        }

        public void Remove(int pointId)
        {
            --m_diff;
            ++m_removed;
            //Debug.LogFormat("Removing point {0}", pointId);
            Debug.AssertFormat(m_observations.ContainsKey(pointId), "[Remove] Container doesn't contain point with id {0}", pointId);
            m_trianglesCloud.SetTetrahedron(pointId, Matrix4x4.identity, new Color(0, 0, 0, 0));
            m_observations.Remove(pointId);
        }

        public void Remove(SlamObservation observation)
        {
            Remove(observation.id);
        }

        public void Clear()
        {
            int[] pointsIds = m_observations.Keys.ToArray();
            for (int i = 0; i < pointsIds.Length; ++i)
            {
                Remove(pointsIds[i]);
            }
            m_observations.Clear();
            m_trianglesCloud.Clear();
            Repaint();

            Debug.LogFormat("[Clear] Added points: {0}; Removed points: {1}; Diff: {2}", m_added, m_removed, m_diff);
            m_added = 0;
            m_removed = 0;
        }

        public SlamObservation[] GetAll()
        {
            return m_observations.Select(kv => kv.Value).ToArray();
        }

        public void Set(SlamObservation observation)
        {
            SlamObservation buttPlug;
            if (!TryGet(observation, out buttPlug))
            {
                Add(observation);
            }
            else
            {
                Update(observation);
            }
        }

        public SlamObservation Get(int observationId)
        {
            //Debug.AssertFormat(m_points.ContainsKey(pointId), "[Get point] Container doesn't contain point with id {0}", pointId);
            if (!m_observations.ContainsKey(observationId))
            {
                Debug.LogWarningFormat("[Get point] Container doesn't contain point with id {0}", observationId);
                return null;
            }

            return m_observations[observationId];
        }

        public SlamObservation Get(SlamObservation observation)
        {
            return Get(observation.id);
        }

        public bool Exists(int observationId)
        {
            //return m_pointCloud.PointExists(pointId);
            return m_observations.ContainsKey(observationId);
        }

        public bool Exists(SlamObservation observation)
        {
            return Exists(observation.id);
        }

        public bool TryGet(SlamObservation observation, out SlamObservation current)
        {
            current = null;
            if (m_trianglesCloud.TetrahedronExists(observation.id))
            {
                current = Get(observation.id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Repaint()
        {
            m_trianglesCloud.Repaint();
        }
    }
}
