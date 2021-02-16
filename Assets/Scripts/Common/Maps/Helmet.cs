using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Maps
{
    public class Helmet : MonoBehaviour
    {
        public SlamLinesContainer Track;

        public Color color = Color.red;
        public int id;

        Vector3 m_lastPosition;
        Vector3 m_currentPosition;
        int m_trackStep;

        private void Awake()
        {
            Track.Renderer = LineCloudRenderer.StaticRenderer;
        }

        private void OnEnable()
        {
            m_trackStep = 0;
            m_lastPosition = transform.position;
            m_currentPosition = transform.position;
            Track.Clear();
        }

        private void OnDisable()
        {
            ResetHelmet();
        }

        private void UnsafeResetHelmet()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            m_trackStep = 0;
            m_lastPosition = transform.position;
            m_currentPosition = transform.position;
            Track.Clear();
        }

        public void ResetHelmet() =>
            MainThreadInvoker.Instance.Enqueue(UnsafeResetHelmet);

        bool CheckTransformChanged()
        {
            if (transform.hasChanged)
            {
                m_lastPosition = m_currentPosition;
                m_currentPosition = transform.position;
                transform.hasChanged = false;
                return true;
            }
            return false;
        }

        private void UnsafeIncrementTrack()
        {
            CheckTransformChanged();
            var line = new SlamLine(
                new SlamPoint(m_trackStep, m_lastPosition, color),
                new SlamPoint(++m_trackStep, m_currentPosition, color));
            Track.Add(line);
        }

        public void IncrementTrack() => 
            MainThreadInvoker.Instance.Enqueue(UnsafeIncrementTrack);

        private void UnsafeDecrementTrack()
        {
            CheckTransformChanged();
            int prevStepId = m_trackStep - 1;
            int currentStepId = m_trackStep;
            Track.Remove(prevStepId, currentStepId);
            --m_trackStep;
        }

        public void DecrementTrack() =>
            MainThreadInvoker.Instance.Enqueue(UnsafeDecrementTrack);

        // Memento pattern
        public SlamLine[] GetTrackState() => Track.ToArray();

        public void RestoreTrackState(IList<SlamLine> track)
        {
            Track.Clear();
            if (track != null && track.Count != 0)
            {
                foreach (var l in track)
                {
                    Track.Add(l);
                }
                m_lastPosition = track[track.Count - 1].pt1.position;
                m_lastPosition = track[track.Count - 1].pt2.position;
                m_trackStep = track[track.Count - 1].pt2.id;
            }
        }
    }
}