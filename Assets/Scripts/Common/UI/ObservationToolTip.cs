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
        public GameObjectsContainer<SlamObservation> Observations;
        
        private Camera m_camera;

        void Start()
        {
            m_camera = Camera.main;
            floatingViewer.Observations = Observations;
            pinnedViewer.Observations = Observations;
        }

        private void Update()
        {
            var ray = m_camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.transform.CompareTag("Observation"))
            {
                var id = hitInfo.transform.GetComponent<IdContainer>().Id;
                if (Input.GetMouseButton(0))
                {
                    pinnedViewer.ShowObservation(id);
                }
                else
                {
                    floatingViewer.ShowObservation(id);
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