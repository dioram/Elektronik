using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.Windows;
using UnityEngine;

namespace Elektronik.UI
{
    public class ObservationToolTip : MonoBehaviour
    {
        public GameObjectCloud<SlamObservation> ObservationsCloud;
        public bool Enable3DImages { get; set; }
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
            if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.transform.CompareTag("Observation"))
            {
                var data = hitInfo.transform.GetComponent<DataComponent<SlamObservation>>();
                if (Input.GetMouseButtonUp(0))
                {
                    CreateOrShowWindow(data, "Observation #{0}");
                    var image3D = data.GetComponent<Observation3DImage>();
                    if (Enable3DImages && image3D != null)
                    {
                        image3D.enabled = !image3D.enabled;
                    }
                }
                else
                {
                    if (_floatingViewer.gameObject.activeInHierarchy) return;
                    _floatingViewer.Render(data);
                    _floatingViewer.transform.position = Input.mousePosition;
                }
            }
            else
            {
                if (_floatingViewer is { }) _floatingViewer.Hide();
            }
        }

        private void CreateOrShowWindow(DataComponent<SlamObservation> data, string title)
        {
            var v = _pinnedViewers.FirstOrDefault(w => w.ObservationContainer == data.Container.GetHashCode()
                                                          && w.ObservationId == data.Data.Id);

            if (v is null)
            {
                data.Container.OnRemoved += DestroyObsoleteWindows;
                Manager.CreateWindow<ObservationViewer>(title, (viewer, window) =>
                                                        {
                                                            viewer.Render(data);
                                                            _pinnedViewers.Add(
                                                                window.GetComponent<ObservationViewer>());
                                                            viewer.ObservationsCloud = ObservationsCloud;
                                                        },
                                                        new List<object> {data.Data.Id});
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
                    MainThreadInvoker.Instance.Enqueue(() =>
                    {
                        Destroy(v.gameObject);
                        _pinnedViewers.Remove(v);
                    });
                }
            }
        }
    }
}