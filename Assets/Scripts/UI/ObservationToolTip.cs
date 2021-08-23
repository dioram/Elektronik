using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Collision;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Threading;
using Elektronik.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elektronik.UI
{
    public class ObservationToolTip : MonoBehaviour
    {
        [SerializeField] private ObservationCollisionCloud CollisionCloud;
        [SerializeField] private WindowsManager Manager;
        [SerializeField] [Range(0, 1f)] private float CollisionCheckTimeout = 0.1f;

        #region Unity events

        void Start()
        {
            _camera = Camera.main;
            Manager.CreateWindow<ObservationViewer>($"", (viewer, window) =>
            {
                _floatingViewer = viewer;
                viewer.Hide();
            });
            StartCoroutine(CheckCollisionCoroutine());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Private

        private Camera _camera;
        private ObservationViewer _floatingViewer;
        private readonly List<ObservationViewer> _pinnedViewers = new List<ObservationViewer>();

        private IEnumerator CheckCollisionCoroutine()
        {
            while (true)
            {
                var mousePosition = Mouse.current.position.ReadValue();
                var ray = _camera.ScreenPointToRay(mousePosition);
                Task.Run(() =>
                {
                    var collision = CollisionCloud.FindCollided(ray);

                    if (collision.HasValue)
                    {
                        MainThreadInvoker.Enqueue(
                            () => ProcessRaycast(collision.Value.container, collision.Value.item, mousePosition));
                    }
                    else
                    {
                        MainThreadInvoker.Enqueue(HideViewer);
                    }
                });
                yield return new WaitForSeconds(CollisionCheckTimeout);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void ProcessRaycast(IContainer<SlamObservation> container, SlamObservation observation,
                                    Vector3 mousePosition)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                CreateOrShowWindow(container, observation, "Observation #{0}");
            }
            else
            {
                if (_floatingViewer.gameObject.activeInHierarchy) return;
                _floatingViewer.Render((container, observation));
                _floatingViewer.transform.position = mousePosition;
            }
        }

        private void HideViewer()
        {
            if (_floatingViewer != null) _floatingViewer.Hide();
        }

        private void CreateOrShowWindow(IContainer<SlamObservation> container, SlamObservation observation,
                                        string title)
        {
            var v = _pinnedViewers.FirstOrDefault(w => w.ObservationContainer == container.GetHashCode()
                                                          && w.ObservationId == observation.Id);

            if (v is null)
            {
                container.OnRemoved += DestroyObsoleteWindows;
                Manager.CreateWindow<ObservationViewer>(title, (viewer, window) =>
                                                        {
                                                            viewer.Render((container, observation));
                                                            _pinnedViewers.Add(
                                                                window.GetComponent<ObservationViewer>());
                                                        },
                                                        observation.Id);
            }
            else
            {
                v.GetComponent<Window>().Show();
            }
        }

        private void DestroyObsoleteWindows(object container, RemovedEventArgs args)
        {
            foreach (var id in args.RemovedIds)
            {
                var v = _pinnedViewers.FirstOrDefault(w => w.ObservationContainer == container.GetHashCode()
                                                              && w.ObservationId == id);
                if (v != null)
                {
                    MainThreadInvoker.Enqueue(() =>
                    {
                        Destroy(v.gameObject);
                        _pinnedViewers.Remove(v);
                    });
                }
            }
        }

        #endregion
    }
}