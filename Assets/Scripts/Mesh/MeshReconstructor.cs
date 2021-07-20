using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Mesh.MeshBuildNative;
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

        public IEnumerable<ISourceTree> Children { get; } = new ISourceTree[0];

        public void Clear()
        {
            OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(new Vector3[0], new Vector3[0], new int[0]));
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
        private bool _isWorking;
        private bool _calculationRequested;

        private void RequestCalculation()
        {
            if (!_isVisible) return;

            if (!_isWorking) Task.Run(() =>
            {
                try
                {
                    CalculateMesh();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            });
            else _calculationRequested = true;
        }

        private void CalculateMesh()
        {
            _isWorking = true;
            
            var points = _points.OrderBy(p => p.Id).ToArray();
            var observations = _observations.ToArray();
            var obsId2Index = new Dictionary<int, int>();
            for (int i = 0; i < observations.Length; i++)
            {
                obsId2Index[observations[i].Id] = i;
            }

            var connectionsNotSet = true;
            var pointsViewsArr = new Dictionary<int, List<int>>();
            foreach (var observation in observations)
            {
                foreach (var id in observation.ObservedPoints.ToArray())
                {
                    if (!_points.Contains(id)) continue;
                    if (!pointsViewsArr.ContainsKey(id)) pointsViewsArr.Add(id, new List<int>());
                    pointsViewsArr[id].Add(obsId2Index[observation.Point.Id]);
                    connectionsNotSet = false;
                }
            }

            if (connectionsNotSet)
            {
                _isWorking = false;
                return;
            }

            var cPoints = new vectorv(points.Select(p => p.ToNative()));
            var cViews = new vectori2d(points.Select(p => pointsViewsArr.ContainsKey(p.Id)
                                                             ? new vectori(pointsViewsArr[p.Id].OrderBy(i => i))
                                                             : new vectori()));
            var cObservations = new vectort(observations.Select(o => o.ToNative()));

            var builder = new MeshBuilder();
            var output = builder.FromPointsAndObservations(cPoints, cViews, cObservations);

            var vertices = output.points.Select(p => p.ToUnity()).ToArray();
            var normals = output.points.Select(p => p.ToUnity()).ToArray();

            if (_isVisible)
            {
                OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(vertices, normals, output.triangles.ToArray()));

                if (_calculationRequested) CalculateMesh();
                _calculationRequested = false;
            }
            _isWorking = false;
        }

        #endregion
    }

    internal static class CastExtensions
    {
        public static NativeVector ToNative(this SlamPoint point)
            => new NativeVector(point.Position.x, point.Position.y, point.Position.z);

        public static NativeTransform ToNative(this SlamObservation observation)
        {
            var m = Matrix4x4.Rotate(observation.Rotation);
            return new NativeTransform
            {
                position = observation.Point.ToNative(),
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

        public static Vector3 ToUnity(this NativeVector vector) => new Vector3(vector.x, vector.y, vector.z);
    }
}