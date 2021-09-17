using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.Containers.NativeMesh;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    public class MeshReconstructor : IMeshContainer
    {
        public MeshReconstructor(IContainer<SlamPoint> points, string displayName = "Mesh")
        {
            _points = points;
            DisplayName = displayName;
            _points.OnAdded += (_, __) => RequestCalculation();
            _points.OnUpdated += (_, __) => RequestCalculation();
            _points.OnRemoved += (_, __) => RequestCalculation();
        }

        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;

        public void OverrideColors()
        {
            foreach (var renderer in _renderer)
            {
                renderer.OverrideColors = !renderer.OverrideColors;
            }
        }

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTreeNode> Children { get; } = Array.Empty<ISourceTreeNode>();

        public void Clear()
        {
            OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(Array.Empty<(Vector3, Color)>(),
                                                                 Array.Empty<int>()));
        }

        public void AddConsumer(IDataConsumer consumer)
        {
            if (!(consumer is IMeshRenderer meshRenderer)) return;
            _renderer.Add(meshRenderer);
            OnMeshUpdated += meshRenderer.OnMeshUpdated;
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            if (!(consumer is IMeshRenderer meshRenderer)) return;
            _renderer.Remove(meshRenderer);
            OnMeshUpdated -= meshRenderer.OnMeshUpdated;
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;

                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);

                if (_isVisible) RequestCalculation();
                else Clear();
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region Private

        private bool _isVisible = false;
        private readonly IContainer<SlamPoint> _points;
        private readonly ThreadWorkerSingleAwaiter _threadWorker = new ThreadWorkerSingleAwaiter();
        private readonly List<IMeshRenderer> _renderer = new List<IMeshRenderer>();

        private void RequestCalculation()
        {
            if (!_isVisible) return;

            _threadWorker.Enqueue(CalculateMesh);
        }

        private static (Vector3 position, Color color) ToUnity(NativePoint point) =>
                (new Vector3(point.x, point.y, point.z), new Color(point.r, point.g, point.b));

        private void CalculateMesh()
        {
            var points = _points.ToArray();
            var builder = new MeshBuilder();
            var output = builder.FromPoints(points);
            var outputPoints = output.points.Select(ToUnity).ToArray();
            if (_isVisible)
            {
                OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(outputPoints, output.triangles.ToArray()));
            }
        }

        #endregion
    }
}