using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Mesh.Native;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.Containers
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
            _renderer.OverrideColors = !_renderer.OverrideColors;
        }

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
                _renderer = meshRenderer;
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
        private readonly ThreadWorkerSingleAwaiter _threadWorker = new ThreadWorkerSingleAwaiter();
        private IMeshRenderer _renderer;

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