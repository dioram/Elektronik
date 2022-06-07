using System.Collections;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Elektronik.Cameras
{
    /// <summary> This component controls widget that shows camera orientation. </summary>
    internal sealed class CameraOrientationWidget2D : MonoBehaviour
    {
        #region Editor fields

        /// <summary> Time interval for camera alignment animation. </summary>
        [Header("Settings")] [Range(0, 5)] [SerializeField] [Tooltip("Time interval for camera alignment animation.")]
        private float RotationTime = 1;

        /// <summary> Length of axis lines in widget. </summary>
        [Range(0, 100)] [SerializeField] [Tooltip("Length of axis lines in widget.")]
        private float Scale = 15;

        /// <summary> Size of marker with axis name. </summary>
        [Range(0, 30)] [SerializeField] [Tooltip("Size of marker with axis name.")]
        private float MarkerSize = 20;

        [SerializeField] private bool ShowCoordinates = true;

        [Header("Objects")] [SerializeField] private RectTransform[] AxisLabels = new RectTransform[6];
        [SerializeField] private TMP_Text PositionLabel;
        [SerializeField] private UILineRenderer XLine;
        [SerializeField] private UILineRenderer YLine;
        [SerializeField] private UILineRenderer ZLine;

        #endregion

        #region Unity events

        private void Start()
        {
            _camera = Camera.main.transform;
            ShowCoordinates = ShowCoordinates && (PositionLabel != null);

            var axes = new[]
            {
                Vector3.forward, Vector3.back, Vector3.down, Vector3.up, Vector3.left, Vector3.right,
            };

            AxisLabels.Select(l => l.GetComponent<Button>())
                    .Select((button, i) => button.OnClickAsObservable().Select(_ => axes[i]))
                    .Merge()
                    .Subscribe(a => StartCoroutine(RotateCamera(a)))
                    .AddTo(this);
        }

        private void Update()
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

            for (var i = 0; i < 6; i++)
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

        #endregion

        #region Private

        private Transform _camera;
        private bool _rotating;

        private void UpdateCoordinatesLabel()
        {
            if (PositionLabel == null) return;
            PositionLabel.gameObject.SetActive(ShowCoordinates);
            if (!ShowCoordinates) return;
            var position = _camera.position;
            PositionLabel.text = $"({position.x:f2}, {position.y:f2}, {position.z:f2})";
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

        #endregion
    }
}