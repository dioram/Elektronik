using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elektronik.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class Ruler : MonoBehaviour
    {
        public Vector3 Offset = new Vector3(0, 0.2f, 0);
        public Vector3 FirstPoint;

        public event Action<Ruler> Destroyed;

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
                _renderer.SetPosition(1, SecondPoint);
                Label.gameObject.SetActive(true);
                Label.transform.position = (SecondPoint + FirstPoint) / 2 + Offset;
            }
        }

        [SerializeField] private Transform DeleteButton;
        [SerializeField] private TMP_Text Label;
        
        private LineRenderer _renderer;
        private Camera _camera;
        private float _distance;
        private bool _isSecondPointSet = false;
        private Vector3 _secondPoint;

        public float Distance
        {
            get => _distance;
            set
            {
                if (Math.Abs(_distance - value) < float.Epsilon) return;
                _distance = value;
                Label.text = $"{_distance:F2}m.";
            }
        }

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

        private void Start()
        {
            _renderer = GetComponent<LineRenderer>();
            _camera = Camera.main;
            _renderer.SetPosition(0, FirstPoint);
        }

        private void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            if (!_isSecondPointSet)
            {
                var pos = new Vector3(mousePos.x, mousePos.y, 1);
                _renderer.SetPosition(1, _camera.ScreenToWorldPoint(pos));
            }
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}