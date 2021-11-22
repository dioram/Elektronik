using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.Containers.NativeMesh;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    /// <summary> This class reconstructs mesh by from given points cloud. </summary>
    public class MeshReconstructor : IMeshContainer
    {
        /// <summary> Constructor. </summary>
        /// <param name="cloud"> Container of points. </param>
        /// <param name="displayName"> Name that will be displayed in tree. </param>
        public MeshReconstructor(ICloudContainer<SlamPoint> cloud, string displayName = "Mesh")
        {
            _cloud = cloud;
            DisplayName = displayName;
            _cloud.OnAdded += (_, __) => RequestCalculation();
            _cloud.OnUpdated += (_, __) => RequestCalculation();
            _cloud.OnRemoved += (_, __) => RequestCalculation();
        }

        #region IMeshContainer

        /// <inheritdoc />
        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;

        /// <inheritdoc />
        public void SwitchShader()
        {
            foreach (var renderer in _renderer)
            {
                renderer.ShaderId++;
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
            OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(Array.Empty<(Vector3, Color)>(),
                                                                 Array.Empty<int>()));
        }

        /// <inheritdoc />
        public void AddConsumer(IDataConsumer consumer)
        {
            if (!(consumer is IMeshRenderer meshRenderer)) return;
            _renderer.Add(meshRenderer);
            OnMeshUpdated += meshRenderer.OnMeshUpdated;
        }

        /// <inheritdoc />
        public void RemoveConsumer(IDataConsumer consumer)
        {
            if (!(consumer is IMeshRenderer meshRenderer)) return;
            _renderer.Remove(meshRenderer);
            OnMeshUpdated -= meshRenderer.OnMeshUpdated;
        }

        /// <inheritdoc />
        public IDataSource TakeSnapshot() => null;

        #endregion

        #region IVisibleDataSource

        /// <inheritdoc />
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

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Private

        private bool _isVisible = false;
        private readonly ICloudContainer<SlamPoint> _cloud;
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
            var points = _cloud.ToArray();
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