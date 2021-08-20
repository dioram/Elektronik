using System;
using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using System.Linq;
using Elektronik.Mesh.Native;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshReconstructor : IMeshContainer
    {
        public MeshReconstructor(IContainer<SlamPoint> points, IContainer<SlamObservation> observations,
                                 string displayName = "Mesh")
        {
            _points = points;
            _observations = observations;
            DisplayName = displayName;
            _points.OnAdded += (_, __) => RequestCalculation();
            _points.OnUpdated += (_, __) => RequestCalculation();
            _points.OnRemoved += (_, __) => RequestCalculation();
            _observations.OnAdded += (_, __) => RequestCalculation();
            _observations.OnUpdated += (_, __) => RequestCalculation();
            _observations.OnRemoved += (_, __) => RequestCalculation();
        }

        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; } = Array.Empty<ISourceTree>();

        public void Clear()
        {
            OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(Array.Empty<(Vector3, Color)>(),
                                                                 Array.Empty<int>()));
        }

        public void SetRenderer(ISourceRenderer renderer)
        {
            if (renderer is IMeshRenderer meshRenderer)
            {
                OnMeshUpdated += meshRenderer.OnMeshUpdated;
            }
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
        private readonly IContainer<SlamObservation> _observations;
        private readonly ThreadWorkerSingleAwaiter _threadWorker = new ThreadWorkerSingleAwaiter();

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