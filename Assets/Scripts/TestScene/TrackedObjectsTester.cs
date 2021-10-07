using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataControllers;
using Elektronik.DataSources.Containers;
using UnityEngine;

namespace Elektronik.TestScene
{
    internal class TrackedObjectsTester : MonoBehaviour
    {
        #region Editor fields

        public DataSourcesController Controller;

        #endregion

        #region Unity events

        private void Start()
        {
            _trackedObjects = _trackedObjects
                    .Select(t => new SlamTrackedObject(t.Id, t.Position + transform.position, t.Rotation, t.Color))
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

        private SlamTrackedObject[] _trackedObjects =
        {
            new SlamTrackedObject(0, Vector3.zero, Quaternion.identity, Color.red),
            new SlamTrackedObject(1, Vector3.left, Quaternion.identity, Color.blue),
            new SlamTrackedObject(2, Vector3.right, Quaternion.identity, Color.green),
        };

        private readonly TrackedObjectsContainer _container = new TrackedObjectsContainer();

        private IEnumerator UpdateContainer()
        {
            yield return new WaitForSeconds(1);
            while (true)
            {
                _container.AddRange(_trackedObjects);
                yield return new WaitForSeconds(0.5f);
                _container.Update(_trackedObjects.Select(Rotated).ToArray());
                yield return new WaitForSeconds(0.5f);
                _container.Update(_trackedObjects.Select(t => new SlamTrackedObject(t.Id, t.Position + Vector3.forward, t.Rotation, t.Color)).ToArray());
                yield return new WaitForSeconds(0.5f);
                _container.Update(_trackedObjects.Select(t => new SlamTrackedObject(t.Id, t.Position, t.Rotation, Color.black)).ToArray());
                yield return new WaitForSeconds(0.5f);
                _container.Remove(new List<int> { 1 });
                yield return new WaitForSeconds(0.5f);
                _container.Clear();
                yield return new WaitForSeconds(0.5f);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static SlamTrackedObject Rotated(SlamTrackedObject point)
        {
            return new SlamTrackedObject(point.Id, point.Position, Quaternion.AngleAxis(45, Vector3.forward), point.Color);
        }

        #endregion
    }
}