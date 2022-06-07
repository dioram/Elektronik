using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataControllers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using UnityEngine;

namespace Elektronik.TestScene
{
    internal class ConnectedPointsTester : MonoBehaviour
    {
        #region Editor fields

        public DataSourcesController Controller;

        #endregion

        #region Unity events

        private void Start()
        {
            _points = _points.Select(p => new SlamPoint(p.Id, p.Position + transform.position, p.Color))
                    .ToArray();
            Controller.AddDataSource(_cloudContainer);
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
        
        private SlamPoint[] _points =
        {
            new SlamPoint(0, Vector3.up, Color.red),
            new SlamPoint(1, Vector3.forward, Color.blue),
            new SlamPoint(2, new Vector3(Mathf.Sqrt(2)/2, 0, -Mathf.Sqrt(2)/2), Color.green),
            new SlamPoint(3, new Vector3(-Mathf.Sqrt(2)/2, 0, -Mathf.Sqrt(2)/2), Color.yellow),
        };

        private readonly (int id1, int id2)[] _connections = { (0, 1), (0, 2), (0, 3), (1, 2), (1, 3), (2, 3) };

        private readonly ConnectableObjectsCloudContainer<SlamPoint> _cloudContainer =
                new ConnectableObjectsCloudContainer<SlamPoint>(new CloudContainer<SlamPoint>(), new SlamLinesCloudContainer());

        private IEnumerator UpdateContainer()
        {
            yield return new WaitForSeconds(1);
            while (true)
            {
                _cloudContainer.AddRange(_points);
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.AddConnections(_connections);
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Update(_points.Select(Rotated).ToArray());
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Update(_points.Select(p => new SlamPoint(p.Id, p.Position, Color.black)).ToArray());
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.RemoveConnections(new []{(0, 1), (0, 2)});
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Remove(new List<int> { 1, 2 });
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Clear();
                yield return new WaitForSeconds(0.5f);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static SlamPoint Rotated(SlamPoint point)
        {
            return new SlamPoint(point.Id, Quaternion.AngleAxis(45, Vector3.forward) * point.Position, point.Color);
        }

        #endregion
    }
}