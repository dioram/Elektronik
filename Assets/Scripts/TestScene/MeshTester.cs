using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataControllers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Elektronik.TestScene
{
    internal class MeshTester : MonoBehaviour
    {
        #region Editor fields

        public DataSourcesController Controller;
        [Range(0, 10)] public float Side = 3;
        [Range(0, 0.3f)] public float Noise = 0.01f;
        [Range(0, 200)] public int Amount = 50;

        #endregion

        #region Unity events

        private void Start()
        {
            _cloudContainer = new CloudContainer<SlamPoint>();
            _reconstructor = new MeshReconstructor(_cloudContainer) { IsVisible = true };
            Controller.AddDataSource(_cloudContainer);
            Controller.AddDataSource(_reconstructor);

            var offset = transform.position;
            _cloudContainer.AddRange(GenerateCube(GeneratePlaneOfPoints(Amount, Side, offset, Noise), Side, offset));

            Observable.Interval(TimeSpan.FromSeconds(0.5), Scheduler.ThreadPool)
                    .Do(_ => _cloudContainer.Update(Rotate(_cloudContainer).ToList()))
                    .Subscribe()
                    .AddTo(this);
        }

        #endregion

        #region Private

        private MeshReconstructor _reconstructor;
        private CloudContainer<SlamPoint> _cloudContainer;

        private IEnumerable<SlamPoint> Rotate(IEnumerable<SlamPoint> points)
        {
            return points.Select(point => new SlamPoint(point.Id,
                                                        Quaternion.AngleAxis(5, Vector3.forward) * point.Position,
                                                        point.Color));
        }


        private SlamPoint[] GeneratePlaneOfPoints(int amount, float side, Vector3 offset, float noise = 0)
        {
            return Enumerable.Range(0, amount)
                    .Select(id => new SlamPoint(id, new Vector3(Random.value * side - side / 2,
                                                                Random.value * noise - noise / 2,
                                                                Random.value * side - side / 2) + offset, Color.blue))
                    .ToArray();
        }

        private SlamPoint[] GenerateCube(SlamPoint[] plane, float side, Vector3 offset)
        {
            return new List<IEnumerable<SlamPoint>>
            {
                plane.Select(s => new SlamPoint
                {
                    Id = s.Id, 
                    Color = Color.cyan,
                    Position = s.Position + Vector3.up * side / 2
                }),
                plane.Select(s => new SlamPoint
                {
                    Id = s.Id + plane.Length,
                    Color = Color.red,
                    Position = Quaternion.AngleAxis(90, Vector3.forward) * (s.Position - offset) + offset + Vector3.right * side / 2,
                }),
                plane.Select(s => new SlamPoint
                {
                    Id = s.Id + plane.Length * 2, 
                    Color = Color.blue,
                    Position = Quaternion.AngleAxis(90, Vector3.back) * (s.Position - offset) + offset + Vector3.left * side / 2,
                }),
                plane.Select(s => new SlamPoint
                {
                    Id = s.Id + plane.Length * 3, 
                    Color = Color.yellow,
                    Position = Quaternion.AngleAxis(90, Vector3.left) * (s.Position - offset) + offset + Vector3.forward * side / 2,
                }),
                plane.Select(s => new SlamPoint
                {
                    Id = s.Id + plane.Length * 4, 
                    Color = Color.green,
                    Position = Quaternion.AngleAxis(90, Vector3.right) * (s.Position - offset) + offset + Vector3.back * side / 2,
                }),
                plane.Select(s => new SlamPoint
                {
                    Id = s.Id + plane.Length * 5, 
                    Color = Color.magenta,
                    Position = s.Position + Vector3.down * side / 2,
                }),
            }.SelectMany(p => p).ToArray();
        }

        #endregion
    }
}