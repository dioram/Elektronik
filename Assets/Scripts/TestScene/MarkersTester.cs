using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataControllers;
using Elektronik.DataSources.Containers;
using UnityEngine;

namespace Elektronik.TestScene
{
    public class MarkersTester : MonoBehaviour
    {
        #region Editor fields

        public DataSourcesController Controller;

        #endregion

        #region Unity events

        private void Start()
        {
            _markers = _markers
                    .Select(m => new SlamMarker(m.Id, m.Position + transform.position, m.Rotation, m.Scale, m.Color,
                                                m.Message, m.Type))
                    .ToArray();
            Controller.AddDataSource(_container);
        }

        private void OnEnable()
        {
            StartCoroutine(UpdateContainer());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Private

        private SlamMarker[] _markers =
        {
            new SlamMarker(0, Vector3.up * 2, Quaternion.identity, Vector3.one, Color.red, "first",
                           SlamMarker.MarkerType.Cube),
            new SlamMarker(1, Vector3.down * 2, Quaternion.identity, Vector3.one, Color.blue, "second",
                           SlamMarker.MarkerType.SemitransparentCube),
        };

        private readonly CloudContainer<SlamMarker> _container = new CloudContainer<SlamMarker>();

        private IEnumerator UpdateContainer()
        {
            yield return new WaitForSeconds(1);
            while (true)
            {
                _container.AddRange(_markers);
                yield return new WaitForSeconds(0.5f);
                _container.Update(_markers.Select(Rotated).ToArray());
                yield return new WaitForSeconds(0.5f);
                _container.Update(_markers.Select(ChangeMessage).ToArray());
                yield return new WaitForSeconds(0.5f);
                _container.Update(_markers.Select(ChangePosition).ToArray());
                yield return new WaitForSeconds(0.5f);
                _container.Update(_markers.Select(ChangeColor).ToArray());
                yield return new WaitForSeconds(0.5f);
                _container.Update(SwapTypes());
                yield return new WaitForSeconds(0.5f);
                _container.Remove(new List<int> { 1 });
                yield return new WaitForSeconds(0.5f);
                _container.Clear();
                yield return new WaitForSeconds(0.5f);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static SlamMarker Rotated(SlamMarker marker)
        {
            return new SlamMarker(marker.Id, marker.Position, Quaternion.AngleAxis(45, Vector3.forward), Vector3.one,
                                  marker.Color, marker.Message, marker.Type);
        }

        private static SlamMarker ChangeMessage(SlamMarker marker)
        {
            return new SlamMarker(marker.Id, marker.Position, marker.Rotation, Vector3.one,
                                  marker.Color, $"#{marker.Id} ({marker.Type})", marker.Type);
        }

        private static SlamMarker ChangePosition(SlamMarker marker)
        {
            return new SlamMarker(marker.Id, marker.Position + Vector3.forward, marker.Rotation,
                                  marker.Scale, marker.Color, marker.Message, marker.Type);
        }

        private static SlamMarker ChangeColor(SlamMarker marker)
        {
            return new SlamMarker(marker.Id, marker.Position, marker.Rotation,
                                  marker.Scale, Color.black, marker.Message, marker.Type);
        }

        private SlamMarker[] SwapTypes()
        {
            var m = _markers.ToArray();
            m[0].Type = SlamMarker.MarkerType.SemitransparentCube;
            m[1].Type = SlamMarker.MarkerType.Cube;
            return m;
        }

        #endregion
    }
}