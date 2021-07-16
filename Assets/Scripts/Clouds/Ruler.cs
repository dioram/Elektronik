using System;
using TMPro;
using UnityEngine;

namespace Elektronik.Clouds
{
    [RequireComponent(typeof(LineRenderer))]
    public class Ruler : MonoBehaviour
    {
        public Vector3 Offset = new Vector3(0, 0.2f, 0);
        public Vector3 FirstPoint;

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

        [SerializeField] private TMP_Text Label;
        
        private LineRenderer _renderer;
        private Camera _camera;
        private float _distance;
        private bool _isSecondPointSet = false;
        private bool _destroyStarted = false;
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

        private void Start()
        {
            _renderer = GetComponent<LineRenderer>();
            _camera = Camera.main;
            _renderer.SetPosition(0, FirstPoint);
        }

        private void Update()
        {
            if (!_isSecondPointSet)
            {
                var pos = Input.mousePosition;
                pos.z = 1;
                _renderer.SetPosition(1, _camera.ScreenToWorldPoint(pos));
            }
            else if (!_destroyStarted)
            {
                Destroy(gameObject, 10f);
                _destroyStarted = true;
            }
        }
    }
}