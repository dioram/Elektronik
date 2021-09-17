using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.Containers.SpecialInterfaces;
using Elektronik.Threading;

namespace Elektronik.DataSources.Containers
{
    public class Connector : ISourceTreeNode, IVisible, IWeightable
    {
        public Connector(IContainer<SlamPoint> points, IContainer<SlamObservation> observations, string displayName)
        {
            _observations = observations;
            points.OnRemoved += OnPointsRemoved;
            _observations.OnAdded += OnObservationsAdded;
            _observations.OnUpdated += OnObservationsUpdated;
            _observations.OnRemoved += OnObservationsRemoved;
            _connections.OnVisibleChanged += OnVisibleChanged;
            DisplayName = displayName;
        }

        #region IWeightable

        public int MaxWeight
        {
            get => _maxWeight;
            private set
            {
                if (_maxWeight == value) return;
                _maxWeight = value;
                MainThreadInvoker.Enqueue(() => OnMaxWeightChanged?.Invoke(_maxWeight));
            }
        }

        private int _maxWeight = 0;

        public event Action<int> OnMaxWeightChanged;

        public int MinWeight
        {
            get => _minWeight;
            set
            {
                if (value > _minWeight)
                {
                    OnMinWeightIncreased(value);
                    _minWeight = value;
                }
                else if (value < _minWeight)
                {
                    OnMinWeightDecreased(value);
                    _minWeight = value;
                }
            }
        }

        #endregion

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTreeNode> Children { get; } = Array.Empty<ISourceTreeNode>();

        public void Clear()
        {
            _connections.Clear();
            lock (_weights)
            {
                _weights.Clear();
            }
        }

        public void AddConsumer(IDataConsumer consumer)
        {
            _connections.AddConsumer(consumer);
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            _connections.RemoveConsumer(consumer);
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _connections.IsVisible;
            set => _connections.IsVisible = value;
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region Private

        private int _minWeight;
        private readonly IContainer<SlamObservation> _observations;
        private readonly SlamLinesContainer _connections = new SlamLinesContainer();

        private readonly Dictionary<(SlamPoint, SlamPoint), int> _weights =
                new Dictionary<(SlamPoint, SlamPoint), int>();

        private (SlamPoint, SlamPoint) GetKey(SlamPoint point1, SlamPoint point2) =>
                point1.Id < point2.Id ? (point1, point2) : (point2, point1);

        private void OnObservationsAdded(object sender, AddedEventArgs<SlamObservation> e)
        {
            var points = new List<(SlamPoint, SlamPoint)>();
            foreach (var observation in _observations)
            {
                lock (_weights)
                {
                    foreach (var added in e.AddedItems)
                    {
                        if (observation.Id == added.Id) continue;
                        if (_weights.ContainsKey(GetKey(observation, added))) continue;
                        if (!observation.ObservedPoints.Overlaps(added.ObservedPoints)) continue;

                        var intersections = new HashSet<int>(observation.ObservedPoints);
                        intersections.IntersectWith(added.ObservedPoints);
                        var weight = intersections.Count;
                        MaxWeight = Math.Max(weight, MaxWeight);
                        var key = GetKey(observation, added);
                        _weights[key] = weight;
                        if (weight > _minWeight) points.Add(key);
                    }
                }
            }

            if (points.Count == 0) return;
            _connections.AddRange(points.Select(p => new SlamLine(p.Item1, p.Item2)).ToArray());
        }

        private void OnObservationsUpdated(object sender, UpdatedEventArgs<SlamObservation> e)
        {
            if (e.UpdatedItems.Count <= 0) return;
            OnObservationsRemoved(sender, new RemovedEventArgs<SlamObservation>(e.UpdatedItems));
            OnObservationsAdded(sender, new AddedEventArgs<SlamObservation>(e.UpdatedItems));
        }

        private void OnObservationsRemoved(object sender, RemovedEventArgs<SlamObservation> e)
        {
            var keys = new List<(SlamPoint, SlamPoint)>();
            lock (_weights)
            {
                foreach (var obs in e.RemovedItems)
                {
                    foreach (var (point1, point2) in _weights.Keys)
                    {
                        if (point1.Id == obs.Id || point2.Id == obs.Id) keys.Add(GetKey(point1, point2));
                    }
                }

                foreach (var key in keys)
                {
                    _weights.Remove(key);
                }
            }

            var toRemove = keys.Select(ids => new SlamLine(ids.Item1, ids.Item2)).ToArray();
            if (toRemove.Length > 0) _connections.Remove(toRemove);
        }

        private void OnPointsRemoved(object sender, RemovedEventArgs<SlamPoint> e)
        {
            var obs = _observations.ToArray();
            var updatedObservations = new List<SlamObservation>();
            foreach (var observation in obs)
            {
                var updated = false;
                foreach (var point in e.RemovedItems)
                {
                    if (!observation.ObservedPoints.Contains(point.Id)) continue;
                    updated = true;
                    observation.ObservedPoints.Remove(point.Id);
                }

                if (updated) updatedObservations.Add(observation);
            }

            if (updatedObservations.Count > 0) _observations.Update(updatedObservations);
        }

        private void OnMinWeightIncreased(float value)
        {
            SlamLine[] toRemove;
            lock (_weights)
            {
                toRemove = _weights.Where(pair => pair.Value >= _minWeight && pair.Value < value)
                        .Select(p => p.Key)
                        .Select(k => new SlamLine(k.Item1, k.Item2))
                        .ToArray();
            }

            _connections.Remove(toRemove);
        }

        private void OnMinWeightDecreased(float value)
        {
            SlamLine[] toAdd;
            lock (_weights)
            {
                toAdd = _weights.Where(pair => pair.Value >= value && pair.Value < _minWeight)
                        .Select(p => p.Key)
                        .Select(k => new SlamLine(k.Item1, k.Item2))
                        .ToArray();
            }
            _connections.AddRange(toAdd);
        }

        #endregion
    }
}