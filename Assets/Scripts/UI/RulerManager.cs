using System.Collections;
using Elektronik.DataConsumers.Collision;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elektronik.UI
{
    /// <summary> This component controls <see cref="Ruler"/>. </summary>
    internal class RulerManager : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private PointCollisionCloud PointCollisionCloud;
        [SerializeField] private  GameObject LineRendererPrefab;
        [SerializeField] private  Transform Cursor;

        #endregion
        
        /// <summary> Property for setting scene scale. </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public float Scale { get; set; } = 1;

        #region Unity events
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!Cursor.gameObject.activeSelf || !Mouse.current.leftButton.wasReleasedThisFrame) return;
            if (_currentRuler is null)
            {
                _currentRuler = Instantiate(LineRendererPrefab, transform).GetComponent<Ruler>();
                _currentRuler.OnDestroyed += OnRulerDestroyed;
                _currentRuler.FirstPoint = Cursor.position;
            }
            else
            {
                _currentRuler.SecondPoint = Cursor.position;
                _currentRuler.OnDestroyed -= OnRulerDestroyed;
                _currentRuler = null;
            }
        }

        private void OnRulerDestroyed(Ruler ruler)
        {
            if (_currentRuler == ruler) _currentRuler = null;
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

        private Camera _camera;
        private Ruler _currentRuler = null;

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
                    Cursor.position = pointData.Value.item.Position * Scale;
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