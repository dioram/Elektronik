using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Elektronik.Cameras
{
    public class CameraOrientationWidget2D : MonoBehaviour
    {
        [Range(0, 5)] public float RotationTime = 1;
        [Range(0, 100)] public float Scale = 15;
        [Range(0, 30)] public float MarkerSize = 20;
        [SerializeField] private RectTransform[] AxisLabels = new RectTransform[6];
        public TMP_Text PositionLabel;
        public UILineRenderer XLine;
        public UILineRenderer YLine;
        public UILineRenderer ZLine;
        public bool ShowCoordinates = true;
        private Transform _camera;
        private bool _rotating;

        private void Start()
        {
            _camera = Camera.main.transform;
            ShowCoordinates = ShowCoordinates && (PositionLabel != null);
            
            AxisLabels[0].GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => StartCoroutine(RotateCamera(Vector3.forward)));
            AxisLabels[1].GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => StartCoroutine(RotateCamera(Vector3.back)));
            AxisLabels[2].GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => StartCoroutine(RotateCamera(Vector3.down)));
            AxisLabels[3].GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => StartCoroutine(RotateCamera(Vector3.up)));
            AxisLabels[4].GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => StartCoroutine(RotateCamera(Vector3.left)));
            AxisLabels[5].GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => StartCoroutine(RotateCamera(Vector3.right)));
        }

        protected void Update()
        {
            UpdateCoordinatesLabel();
            
            var cameraAxis = new[]
            {
                _camera.forward,
                -_camera.forward,
                _camera.up,
                -_camera.up,
                _camera.right,
                -_camera.right,
            };

            XLine.Points[1] = _camera.right * Scale;
            YLine.Points[1] = _camera.up * Scale;
            ZLine.Points[1] = -_camera.forward * Scale;
            // force redraw
            XLine.SetAllDirty();
            YLine.SetAllDirty();
            ZLine.SetAllDirty();

            for (int i = 0; i < 6; i++)
            {
                AxisLabels[i].anchoredPosition = cameraAxis[i] * Scale;
                if (cameraAxis[i].z > 0)
                {
                    AxisLabels[i].SetAsLastSibling();
                    AxisLabels[i].sizeDelta = new Vector2(MarkerSize, MarkerSize);
                }
                else
                {
                    AxisLabels[i].sizeDelta = new Vector2(MarkerSize * 0.75f, MarkerSize * 0.75f);
                }
            }
        }

        private void UpdateCoordinatesLabel()
        {
            if (PositionLabel == null) return;
            PositionLabel.gameObject.SetActive(ShowCoordinates);
            if (!ShowCoordinates) return;
            PositionLabel.text = $"({_camera.position.x:f2}, {_camera.position.y:f2}, {_camera.position.z:f2})";
        }

        private IEnumerator RotateCamera(Vector3 forward)
        {
            if (_rotating) yield break;
            _rotating = true;
            var startRotation = _camera.transform.rotation;
            var endRotation = Quaternion.LookRotation(forward, Vector3.up);
            for (float time = 0; time < 1; time += Time.deltaTime / RotationTime)
            {
                _camera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, time);
                yield return null;
            }

            _rotating = false;
        }
    }
}