using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Containers
{
    public class Connector : ISourceTree, IVisible, IWeightable
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
                else if (value < _minWeight)
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

                _minWeight = value;
            }
        }

        #endregion

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; } = new ISourceTree[0];

        public void Clear()
        {
            lock (_weights)
            {
                _weights.Clear();
            }

            _connections.Clear();
        }

        public void SetRenderer(ISourceRenderer renderer)
        {
            _connections.SetRenderer(renderer);
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

            _connections.AddRange(points.Select(p => new SlamLine(p.Item1, p.Item2)).ToArray());
        }

        private void OnObservationsUpdated(object sender, UpdatedEventArgs<SlamObservation> e)
        {
            var obsWithoutChangedConnections = e.UpdatedItems
                    .Where(o => o.ObservedPoints.Count == 0)
                    .Select(o => o.Point)
                    .ToArray();
            _connections.UpdatePositions(obsWithoutChangedConnections);
            var obsWithChangedConnections = e.UpdatedItems
                    .Where(o => o.ObservedPoints.Count != 0)
                    .ToArray();
            OnObservationsRemoved(sender, new RemovedEventArgs(obsWithChangedConnections.Select(o => o.Id)));
            OnObservationsAdded(sender, new AddedEventArgs<SlamObservation>(obsWithChangedConnections));
        }

        private void OnObservationsRemoved(object sender, RemovedEventArgs e)
        {
            var keys = new List<(SlamPoint, SlamPoint)>();
            lock (_weights)
            {
                foreach (var id in e.RemovedIds)
                {
                    foreach (var key in _weights.Keys)
                    {
                        if (key.Item1.Id == id || key.Item2.Id == id) keys.Add(GetKey(key.Item1, key.Item2));
                    }
                }

                foreach (var key in keys)
                {
                    _weights.Remove(key);
                }
            }

            _connections.Remove(keys.Select(ids => new SlamLine(ids.Item1, ids.Item2)).ToArray());
        }

        private void OnPointsRemoved(object sender, RemovedEventArgs e)
        {
            var obs = _observations.ToArray();
            var updatedObservations = new List<SlamObservation>();
            foreach (var observation in obs)
            {
                var updated = false;
                foreach (var id in e.RemovedIds)
                {
                    if (!observation.ObservedPoints.Contains(id)) continue;
                    updated = true;
                    observation.ObservedPoints.Remove(id);
                }

                if (updated) updatedObservations.Add(observation);
            }

            _observations.Update(updatedObservations);
        }

        #endregion
    }
}