using System.Collections.Generic;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.Windows;
using UnityEngine;

namespace Elektronik.UI
{
    public class ObservationToolTip : MonoBehaviour
    {
        [SerializeField] private WindowsManager Manager;
        private Camera _camera;
        private ObservationViewer _floatingViewer;
        private readonly Dictionary<(int, int), Window> _pinnedViewers = new Dictionary<(int, int), Window>();
        private readonly List<IContainer<SlamObservation>> _containers = new List<IContainer<SlamObservation>>();

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
                var title = $"Observation #{data.Data.Id}";
                if (Input.GetMouseButton(0))
                {
                    CreateOrShowWindow(data, title);
                }
                else
                {
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
            var key = (data.Container.GetHashCode(), data.Data.Id);
            if (!_containers.Contains(data.Container))
            {
                _containers.Add(data.Container);
                data.Container.OnRemoved += DestroyObsoleteWindows;
            }
            if (!_pinnedViewers.ContainsKey(key))
            {
                Manager.CreateWindow<ObservationViewer>(title, (viewer, window) =>
                {
                    viewer.Render(data);
                    _pinnedViewers.Add(key, window);
                });
            }
            else
            {
                _pinnedViewers[key].Show();
            }
        }

        private void DestroyObsoleteWindows(object container, RemovedEventArgs args)
        {
            foreach (var id in args.RemovedIds)
            {
                var key = (container.GetHashCode(), id);
                if (_pinnedViewers.ContainsKey(key))
                {
                    MainThreadInvoker.Instance.Enqueue(() =>
                    {
                        Destroy(_pinnedViewers[key].gameObject);
                        _pinnedViewers.Remove(key);
                    });
                }
            }
        }
    }
}