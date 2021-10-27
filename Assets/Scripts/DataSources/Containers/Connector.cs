using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using UniRx;

namespace Elektronik.DataSources.Containers
{
    /// <summary>
    /// Shows connections between observations if they have intersected <see cref="SlamObservation.ObservedPoints"/>.
    /// </summary>
    public class Connector : IVisibleDataSource, IFilterableDataSource
    {
        /// <summary> Constructor. </summary>
        /// <param name="points"> Container of points. </param>
        /// <param name="observations"> Container of observations. </param>
        /// <param name="displayName"> Name that will be displayed in tree. </param>
        public Connector(ICloudContainer<SlamPoint> points, ICloudContainer<SlamObservation> observations,
                         string displayName)
        {
            _observations = observations;
            points.OnRemoved += OnPointsRemoved;
            _observations.OnAdded += OnObservationsAdded;
            _observations.OnUpdated += OnObservationsUpdated;
            _observations.OnRemoved += OnObservationsRemoved;
            _connections.OnVisibleChanged += OnVisibleChanged;
            DisplayName = displayName;
        }

        #region IFilterableDataSource

        /// <inheritdoc />
        public int MaxWeight
        {
            get => _maxWeight;
            private set
            {
                if (_maxWeight == value) return;
                _maxWeight = value;
                UniRxExtensions.StartOnMainThread(() => OnMaxWeightChanged?.Invoke(_maxWeight)).Subscribe();
            }
        }

        /// <inheritdoc />
        public event Action<int> OnMaxWeightChanged;

        /// <inheritdoc />
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

        #region IDataSource

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public IEnumerable<IDataSource> Children { get; } = Array.Empty<IDataSource>();

        /// <inheritdoc />
        public void Clear()
        {
            _connections.Clear();
            lock (_weights)
            {
                _weights.Clear();
            }
        }

        /// <inheritdoc />
        public void AddConsumer(IDataConsumer consumer)
        {
            _connections.AddConsumer(consumer);
        }

        /// <inheritdoc />
        public void RemoveConsumer(IDataConsumer consumer)
        {
            _connections.RemoveConsumer(consumer);
        }

        /// <inheritdoc />
        /// <remarks> Snapshot of this object cannot be taken. </remarks>
        /// <returns> <c>null</c> </returns>
        public IDataSource TakeSnapshot() => null;

        #endregion

        #region IVisibleDataSource

        /// <inheritdoc />
        public bool IsVisible
        {
            get => _connections.IsVisible;
            set => _connections.IsVisible = value;
        }

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Private

        private int _maxWeight = 0;
        private int _minWeight;
        private readonly ICloudContainer<SlamObservation> _observations;
        private readonly SlamLinesCloudContainer _connections = new SlamLinesCloudContainer();

        private readonly Dictionary<(SlamObservation, SlamObservation), int> _weights =
            new Dictionary<(SlamObservation, SlamObservation), int>();

        private (SlamObservation, SlamObservation) GetKey(SlamObservation point1, SlamObservation point2) =>
            point1.Id < point2.Id ? (point1, point2) : (point2, point1);

        private void OnObservationsAdded(object sender, AddedEventArgs<SlamObservation> e)
        {
            var points = new List<(SlamObservation, SlamObservation)>();
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
            _connections.AddRange(points.Select(p => new SlamLine(p.Item1.ToPoint(), p.Item2.ToPoint())).ToArray());
        }

        private void OnObservationsUpdated(object sender, UpdatedEventArgs<SlamObservation> e)
        {
            if (e.UpdatedItems.Count <= 0) return;
            OnObservationsRemoved(sender, new RemovedEventArgs<SlamObservation>(e.UpdatedItems));
            OnObservationsAdded(sender, new AddedEventArgs<SlamObservation>(e.UpdatedItems));
        }

        private void OnObservationsRemoved(object sender, RemovedEventArgs<SlamObservation> e)
        {
            var keys = new List<(SlamObservation, SlamObservation)>();
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

            var toRemove = keys.Select(ids => new SlamLine(ids.Item1.ToPoint(), ids.Item2.ToPoint())).ToArray();
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
                    .Select(k => new SlamLine(k.Item1.ToPoint(), k.Item2.ToPoint()))
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
                    .Select(k => new SlamLine(k.Item1.ToPoint(), k.Item2.ToPoint()))
                    .ToArray();
            }

            _connections.AddRange(toAdd);
        }

        #endregion
    }
}