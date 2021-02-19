using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Settings;
using UnityEngine;

namespace Elektronik.Common.Maps
{
    public class Helmet : MonoBehaviour
    {
        public SlamLinesContainer Track;

        public Color Color = Color.red;
        public int ID;

        private Vector3 _lastPosition;
        private Vector3 _currentPosition;
        private int _trackStep;

        public void SetActive(bool active)
        {
            GetComponent<XYZAxis>().enabled = active;
            GetComponent<MeshRenderer>().enabled = active;
            Track.enabled = active;
        }

        private void Awake()
        {
            Track.Renderer = LineCloudRenderer.StaticRenderer;
        }

        private void OnEnable()
        {
            _trackStep = 0;
            _lastPosition = transform.position;
            _currentPosition = transform.position;
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
            _trackStep = 0;
            _lastPosition = transform.position;
            _currentPosition = transform.position;
            Track.Clear();
        }

        public void ResetHelmet() =>
            MainThreadInvoker.Instance.Enqueue(UnsafeResetHelmet);

        bool CheckTransformChanged()
        {
            if (transform.hasChanged)
            {
                _lastPosition = _currentPosition;
                _currentPosition = transform.position;
                transform.hasChanged = false;
                return true;
            }
            return false;
        }

        private void UnsafeIncrementTrack()
        {
            CheckTransformChanged();
            var line = new SlamLine(
                new SlamPoint(_trackStep, _lastPosition, Color),
                new SlamPoint(++_trackStep, _currentPosition, Color));
            Track.Add(line);
        }

        public void IncrementTrack() => 
            MainThreadInvoker.Instance.Enqueue(UnsafeIncrementTrack);

        private void UnsafeDecrementTrack()
        {
            CheckTransformChanged();
            int prevStepId = _trackStep - 1;
            int currentStepId = _trackStep;
            Track.Remove(prevStepId, currentStepId);
            --_trackStep;
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
                _lastPosition = track[track.Count - 1].Point1.Position;
                _lastPosition = track[track.Count - 1].Point2.Position;
                _trackStep = track[track.Count - 1].Point2.Id;
            }
        }
    }
}