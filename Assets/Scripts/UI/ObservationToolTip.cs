using System.Collections.Generic;
using System.Linq;
using Elektronik.Collision;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.Windows;
using UnityEngine;

namespace Elektronik.UI
{
    public class ObservationToolTip : MonoBehaviour
    {
        public ObservationCollisionCloud CollisionCloud;
        [SerializeField] private WindowsManager Manager;
        private Camera _camera;
        private ObservationViewer _floatingViewer;
        private readonly List<ObservationViewer> _pinnedViewers = new List<ObservationViewer>();

        void Start()
        {
            _camera = Camera.main;
            Manager.CreateWindow<ObservationViewer>($"", (viewer, window) =>
            {
                _floatingViewer = viewer;
                viewer.Hide();
            });
        }

        private void Update()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var collision = CollisionCloud.FindCollided(ray);
            if (collision.HasValue)
            {
                var (container, observation) = collision.Value;
                if (Input.GetMouseButtonUp(0))
                {
                    CreateOrShowWindow(container, observation, "Observation #{0}");
                }
                else
                {
                    if (_floatingViewer.gameObject.activeInHierarchy) return;
                    _floatingViewer.Render((container, observation));
                    _floatingViewer.transform.position = Input.mousePosition;
                }
            }
            else
            {
                if (_floatingViewer != null) _floatingViewer.Hide();
            }
        }
        
        private void CreateOrShowWindow(IContainer<SlamObservation> container, SlamObservation observation, string title)
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
    }
}