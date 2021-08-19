#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
#define NO_MESH_BUILDER
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using System.Linq;
using Elektronik.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if !NO_MESH_BUILDER
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
            OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(Array.Empty<Vector3>(),
                                                                 Array.Empty<int>(),
                                                                 Array.Empty<Color>()));
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

        private static NativeVector ToNative(SlamPoint point) =>
                new NativeVector(point.Position.x, point.Position.y, point.Position.z);
        
        private static NativeVector GetColors(SlamPoint point) =>
                new NativeVector(point.Color.r, point.Color.g, point.Color.b);

        private static NativeTransform ToNative(SlamObservation observation)
        {
            var m = Matrix4x4.Rotate(observation.Rotation);
            return new NativeTransform
            {
                position = ToNative(observation.Point),
                r11 = m.m00,
                r12 = m.m01,
                r13 = m.m02,
                r21 = m.m10,
                r22 = m.m11,
                r23 = m.m12,
                r31 = m.m20,
                r32 = m.m21,
                r33 = m.m22,
            };
        }

        private static Vector3 ToUnity(NativeVector vector) => new Vector3(vector.x, vector.y, vector.z);

        private void CalculateMesh()
        {
            var w = Stopwatch.StartNew();
            var points = _points.ToArray();

            var cPoints = new vectorv(points.Select(ToNative));
            var cColors = new vectorv(points.Select(GetColors));

            w.Stop();
            var builder = new MeshBuilder();
            var w1 = Stopwatch.StartNew();
            var output = builder.FromPoints(cPoints, cColors);
            w1.Stop();
            var w2 = Stopwatch.StartNew();
            var vertices = output.points.Select(ToUnity).ToArray();
            // TODO: Считать цвет на стороне электроника
            var colors = output.normals.Select(n => new Color(n.x, n.y, n.z)).ToArray();
            w2.Stop();
            Debug.LogError($"Input: {cPoints.Count} points\n" +
                           $"Output: {vertices.Length} points, {colors.Length} colors, {output.triangles.Count / 3} triangles\n" +
                           $"Time: ToNative: {w.ElapsedMilliseconds}ms, Native: {w1.ElapsedMilliseconds}ms, FromNative: {w2.ElapsedMilliseconds}ms.");

            if (_isVisible)
            {
                OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(vertices, output.triangles.ToArray(), colors));
            }
        }

        #endregion
    }
}
#else
namespace Elektronik.Mesh
{
    public class MeshReconstructor : IMeshContainer
    {
        public MeshReconstructor(IContainer<SlamPoint> points, IContainer<SlamObservation> observations,
                                 string displayName = "Mesh")
        {
            throw new NotImplementedException();
        }
        
        public string DisplayName { get; set; }
        public IEnumerable<ISourceTree> Children { get; }
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void SetRenderer(ISourceRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible { get; set; }
        public event Action<bool> OnVisibleChanged;
        public bool ShowButton { get; }
        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;
    }
}
#endif