using TMPro;
using UnityEngine;

namespace Elektronik.Cameras
{
    /// <summary> This component draws grid using GPU. </summary>
    internal sealed class Grid : MonoBehaviour
    {
        #region Editor fields

        /// <summary> Interval between main grid lines. </summary>
        [SerializeField] [Tooltip("Interval between main grid lines.")]
        private float Spacing = 10f;

        /// <summary> Length of main grid line. </summary>
        /// <remarks> For user it should look like infinite line. </remarks>
        [SerializeField] [Tooltip("Length of main grid line.\nFor user it should look like infinite line.")]
        private float MainLineLength = 100;

        /// <summary> Max distance where additional details such as label and small grid will be rendered. </summary>
        [SerializeField]
        [Tooltip("Max distance where additional details such as label and small grid will be rendered.")]
        private float DetailsVisibilityDistance = 50;

        /// <summary> Color of grid lines. </summary>
        [SerializeField] [Tooltip("Color of grid lines.")]
        private Color Color = Color.white;

        /// <summary> Sets how grid should be rendered. </summary>
        [SerializeField] [Tooltip("Sets how grid should be rendered.")]
        private GridModes RenderMode;

        /// <summary> Prefab of coordinates label. </summary>
        [SerializeField] [Tooltip("Prefab of coordinates label.")]
        private GameObject LabelPrefab;

        /// <summary> Offset from gid intersection for coordinates labels. </summary>
        [SerializeField] [Tooltip("Offset from gid intersection for coordinates labels.")]
        private Vector2 LabelOffset = new Vector2(10.5f, 1.5f);

        /// <summary> Length of small grid lines. </summary>
        [SerializeField] [Tooltip("Length of small grid lines.")]
        private float SmallLineLength = 0.5f;

        #endregion

        /// <summary> Modes for grid rendering. </summary>
        public enum GridModes
        {
            /// <summary> Grid will not be rendered. </summary>
            Disabled,
            /// <summary> Grid will be rendered horizontally. </summary>
            EnabledHorizontal,
            /// <summary> Grid will be rendered in plane set by <c>SetPlane()</c> </summary>
            EnabledOrientedGrid,
        }

        /// <summary> Sets plane where to render grid. </summary>
        /// <param name="origin"> Origin of coordinate system. </param>
        /// <param name="yAxis"> Up vector. </param>
        /// <param name="zAxis"> Forward vector. </param>
        public void SetPlane(Vector3 origin, Vector3 yAxis, Vector3 zAxis)
        {
            _position = origin;
            _up = yAxis;
        }

        /// <summary> Sets mode by its index. </summary>
        /// <param name="modeId"></param>
        public void SetMode(int modeId)
        {
            RenderMode = (GridModes)modeId;
        }

        #region Unity events

        private void Awake()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _lineMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt(Cull, (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt(ZWrite, 0);
            _camera = Camera.main;
            _labelsPool = new ObjectPool(LabelPrefab, transform);
        }
        
        // TODO: It is not really necessary but can be rewritten to Graphics.ProceduralDraw
        private void OnRenderObject()
        {
            if (RenderMode == GridModes.Disabled)
            {
                _labelsPool.DespawnAllActiveObjects();
                return;
            }

            _lineMaterial.SetPass(0);

            GL.PushMatrix();
            if (RenderMode == GridModes.EnabledOrientedGrid)
            {
                transform.position = _position;
                transform.up = _up;
                GL.MultMatrix(transform.localToWorldMatrix);
            }
            else
            {
                transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            GL.Begin(GL.LINES);
            GL.Color(Color);
            var pos = transform.worldToLocalMatrix * _camera.transform.position;
            var offset = new Vector2(Mathf.Round(pos.x / Spacing) * Spacing,
                                     Mathf.Round(pos.z / Spacing) * Spacing);

            for (var i = -MainLineLength; i < MainLineLength; i += Spacing)
            {
                DrawGrid(offset, i);
            }

            for (var i = -DetailsVisibilityDistance; i < DetailsVisibilityDistance; i += Spacing)
            {
                for (var j = -DetailsVisibilityDistance; j < DetailsVisibilityDistance; j += Spacing)
                {
                    DrawDetailedGrid(offset + new Vector2(i, j));
                }
            }

            if (_camera.transform.hasChanged)
            {
                TryDrawLabels(offset);
                _camera.transform.hasChanged = false;
            }

            GL.End();
            GL.PopMatrix();
        }

        #endregion

        #region Private

        private static Material _lineMaterial;
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private Camera _camera;
        private ObjectPool _labelsPool;
        private Vector3 _position;
        private Vector3 _up;

        private void TryDrawLabels(Vector2 offset)
        {
            _labelsPool.DespawnAllActiveObjects();
            for (var i = -DetailsVisibilityDistance; i < DetailsVisibilityDistance; i += Spacing)
            {
                for (var j = -DetailsVisibilityDistance; j < DetailsVisibilityDistance; j += Spacing)
                {
                    var visible = IsGridPointVisible(new Vector2(i, j) + offset, DetailsVisibilityDistance);
                    if (!visible) continue;
                    var go = _labelsPool.Spawn();
                    if (RenderMode == GridModes.EnabledOrientedGrid)
                    {
                        go.transform.localPosition = new Vector3(offset.x + i + LabelOffset.x,
                                                                 0,
                                                                 offset.y + j - LabelOffset.y);
                        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        go.transform.position = new Vector3(offset.x + i + LabelOffset.x,
                                                            0,
                                                            offset.y + j - LabelOffset.y);
                        go.transform.rotation = Quaternion.Euler(90, 0, 0);
                    }

                    go.GetComponent<TMP_Text>().text = $"({offset.x + i:F0}, {offset.y + j:F0})";
                }
            }
        }

        private bool IsGridPointVisible(Vector2 gridPoint, float maxDistance)
        {
            var worldPoint = new Vector3(gridPoint.x, 0, gridPoint.y);
            if (RenderMode == GridModes.EnabledOrientedGrid)
            {
                worldPoint = transform.localToWorldMatrix * worldPoint;
            }

            if ((_camera.transform.position - worldPoint).magnitude > maxDistance) return false;
            var cameraPoint = _camera.WorldToViewportPoint(worldPoint);
            if (cameraPoint.z < 0) return false;
            if (cameraPoint.x < 0 || cameraPoint.y < 0) return false;
            if (cameraPoint.x > 1 || cameraPoint.y > 1) return false;
            return true;
        }

        private void DrawDetailedGrid(Vector2 offset)
        {
            GL.Color(Color);
            for (var i = -Spacing; i < Spacing; i += Spacing / 10)
            {
                GL.Vertex3(offset.x + i, 0, offset.y + SmallLineLength);
                GL.Vertex3(offset.x + i, 0, offset.y - SmallLineLength);
                GL.Vertex3(offset.x + SmallLineLength, 0, offset.y + i);
                GL.Vertex3(offset.x - SmallLineLength, 0, offset.y + i);
            }
        }

        private void DrawGrid(Vector2 offset, float i)
        {
            GL.Color(Color * (1 - Mathf.Abs(i / MainLineLength)));
            GL.Vertex3(offset.x, 0, offset.y + i);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x - MainLineLength, 0, offset.y + i);

            GL.Color(Color * (1 - Mathf.Abs(i / MainLineLength)));
            GL.Vertex3(offset.x, 0, offset.y + i);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x + MainLineLength, 0, offset.y + i);

            GL.Color(Color * (1 - Mathf.Abs(i / MainLineLength)));
            GL.Vertex3(offset.x + i, 0, offset.y);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x + i, 0, offset.y - MainLineLength);

            GL.Color(Color * (1 - Mathf.Abs(i / MainLineLength)));
            GL.Vertex3(offset.x + i, 0, offset.y);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x + i, 0, offset.y + MainLineLength);
        }

        #endregion
    }
}