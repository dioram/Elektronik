using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataControllers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using UnityEngine;

namespace Elektronik.TestScene
{
    internal class ConnectedObservationsTester : MonoBehaviour
    {
        #region Editor fields

        public DataSourcesController Controller;

        #endregion

        #region Unity events

        private void Start()
        {
            _observations = _observations
                    .Select(o => new SlamObservation(o.Id, o.Position + transform.position, o.Color,
                                                     o.Rotation, o.Message, o.FileName))
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

        private SlamObservation[] _observations =
        {
            new SlamObservation(0, Vector3.up, Color.red, Quaternion.identity, "", ""),
            new SlamObservation(1, Vector3.forward, Color.blue, Quaternion.identity, "", ""),
            new SlamObservation(2, new Vector3(Mathf.Sqrt(2) / 2, 0, -Mathf.Sqrt(2) / 2), Color.green,
                                Quaternion.identity, "", ""),
            new SlamObservation(3, new Vector3(-Mathf.Sqrt(2) / 2, 0, -Mathf.Sqrt(2) / 2), Color.yellow,
                                Quaternion.identity, "", ""),
        };

        private readonly (int id1, int id2)[] _connections = { (0, 1), (0, 2), (0, 3), (1, 2), (1, 3), (2, 3) };

        private readonly ConnectableObjectsCloudContainer<SlamObservation> _cloudContainer =
            new ConnectableObjectsCloudContainer<SlamObservation>(new CloudContainer<SlamObservation>(),
                                                                  new SlamLinesCloudContainer());

        private IEnumerator UpdateContainer()
        {
            yield return new WaitForSeconds(1);
            while (true)
            {
                _cloudContainer.AddRange(_observations);
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.AddConnections(_connections);
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Update(_observations.Select(Rotated).ToArray());
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Update(_observations.Select(ChangePosition).ToArray());
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Update(_observations.Select(ChangeColor).ToArray());
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.RemoveConnections(new[] { (0, 1), (0, 2) });
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Remove(new List<int> { 1, 2 });
                yield return new WaitForSeconds(0.5f);
                _cloudContainer.Clear();
                yield return new WaitForSeconds(0.5f);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private SlamObservation ChangeColor(SlamObservation obs)
        {
            return new SlamObservation(obs.Id, obs.Position, Color.black, obs.Rotation, obs.Message, obs.FileName);
        }

        private SlamObservation ChangePosition(SlamObservation obs)
        {
            return new SlamObservation(obs.Id, obs.Position + Vector3.up, obs.Color, obs.Rotation, obs.Message,
                                       obs.FileName);
        }

        private static SlamObservation Rotated(SlamObservation obs)
        {
            return new SlamObservation(obs.Id, obs.Position, obs.Color,
                                       Quaternion.AngleAxis(45, Vector3.forward) * obs.Rotation,
                                       obs.Message, obs.Message);
        }

        #endregion
    }
}