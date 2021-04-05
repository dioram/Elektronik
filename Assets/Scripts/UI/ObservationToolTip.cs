using System.Collections.Generic;
using Elektronik.Clouds;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.Windows;
using UnityEngine;

namespace Elektronik.UI
{
    public class ObservationToolTip : MonoBehaviour
    {
        [SerializeField] private WindowsFactory Factory;
        private Camera _camera;
        private ObservationViewer _floatingViewer;
        private readonly List<Window> _pinnedViewers = new List<Window>();

        void Start()
        {
            _camera = Camera.main;
            Factory.CreateWindow<ObservationViewer>($"", (viewer, window) =>
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
                if (Input.GetMouseButton(0) && !_pinnedViewers.Exists(v => v.GetComponent<Window>().Title == title))
                {
                    Factory.CreateWindow<ObservationViewer>(title, (viewer, window) =>
                    {
                        viewer.Render(data);
                        _pinnedViewers.Add(window);
                    });
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
    }
}