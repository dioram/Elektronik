using System.Collections;
using Elektronik.Collision;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elektronik.Clouds
{
    public class RulerManager : MonoBehaviour
    {
        public GameObject LineRendererPrefab;
        [SerializeField] private PointCollisionCloud PointCollisionCloud;
        public Transform Cursor;

        private Camera _camera;
        private Ruler _currentRuler = null;
        
        #region Unity events
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Cursor.gameObject.activeSelf && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (_currentRuler is null)
                {
                    _currentRuler = Instantiate(LineRendererPrefab, transform).GetComponent<Ruler>();
                    _currentRuler.FirstPoint = Cursor.position;
                }
                else
                {
                    _currentRuler.SecondPoint = Cursor.position;
                    _currentRuler = null;
                }
            }
        }

        private void OnEnable()
        {
            StartCoroutine(FindHoveredPoint());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Cursor.gameObject.SetActive(false);
        }

        #endregion

        #region Private

        private IEnumerator FindHoveredPoint()
        {
            while (true)
            {
                var mousePosition = Mouse.current.position.ReadValue();
                var ray = _camera.ScreenPointToRay(mousePosition);
                var pointData = PointCollisionCloud.FindCollided(ray);
                if (pointData.HasValue)
                {
                    Cursor.gameObject.SetActive(true);
                    Cursor.position = pointData.Value.item.Position;
                    Cursor.localScale = Vector3.one * (PointCollisionCloud.Radius * 2.2f);
                }
                else
                {
                    Cursor.gameObject.SetActive(false);
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        #endregion
    }
}