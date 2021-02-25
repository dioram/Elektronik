using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Common.UI
{
    public class ObservationToolTip : MonoBehaviour
    {
        public ObservationViewer floatingViewer;
        public ObservationViewer pinnedViewer;
        public CloudContainer<SlamObservation> Observations;
        
        private Camera _camera;

        void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.transform.CompareTag("Observation"))
            {
                var data = hitInfo.transform.GetComponent<DataComponent<SlamObservation>>().Data;
                if (Input.GetMouseButton(0))
                {
                    pinnedViewer.ShowObservation(data);
                }
                else
                {
                    floatingViewer.ShowObservation(data);
                    floatingViewer.transform.position = Input.mousePosition;
                }
            }
            else
            {
                floatingViewer.Hide();
            }
        }
    }
}