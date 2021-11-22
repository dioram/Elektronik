using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elektronik.UI
{
    /// <summary> This component measures and renders distance between two cloud points. </summary>
    [RequireComponent(typeof(LineRenderer))]
    internal class Ruler : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private Transform DeleteButton;
        [SerializeField] private TMP_Text Label;
        [SerializeField] private Vector3 Offset = new Vector3(0, 0.2f, 0);
        
        #endregion
        
        public Vector3 FirstPoint;

        public event Action<Ruler> OnDestroyed;

        public Vector3 SecondPoint
        {
            get => _secondPoint;
            set
            {
                if (value == FirstPoint)
                {
                    Destroy(gameObject);
                    return;
                }
                
                _secondPoint = value;
                _isSecondPointSet = true;
                Distance = (SecondPoint - FirstPoint).magnitude;
                Label.text = $"{Distance:F2}m.";
                _renderer.SetPosition(1, SecondPoint);
                Label.gameObject.SetActive(true);
                Label.transform.position = (SecondPoint + FirstPoint) / 2 + Offset;
            }
        }
        
        public float Distance { get; private set; }

        public void OnClick()
        {
            var mousePos = Mouse.current.position.ReadValue();
            if (Physics.Raycast(_camera.ScreenPointToRay(mousePos), out var hit)
                && hit.transform == DeleteButton)
            {
                Destroy(gameObject);
            }
        }

        public void OnCancel()
        {
            if (!_isSecondPointSet)
            {
                Destroy(gameObject);
            }
        }

        #region Unity events
        
        private void Start()
        {
            _renderer = GetComponent<LineRenderer>();
            _camera = Camera.main;
            _renderer.SetPosition(0, FirstPoint);
        }

        private void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            if (_isSecondPointSet) return;
            var pos = new Vector3(mousePos.x, mousePos.y, 1);
            _renderer.SetPosition(1, _camera.ScreenToWorldPoint(pos));
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }

        #endregion

        #region Private

        private LineRenderer _renderer;
        private Camera _camera;
        private bool _isSecondPointSet = false;
        private Vector3 _secondPoint;
        
        #endregion
    }
}