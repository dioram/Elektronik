using Elektronik.Data.PackageObjects;
using TMPro;
using UnityEngine;

namespace Elektronik.Cameras
{
    public class Grid : MonoBehaviour
    {
        public float Spacing = 10f;
        public float Length = 100;
        public float DetailsVisibilityDistance = 50;
        public Color Color = Color.white;
        public GridModes Mode;
        public GameObject LabelPrefab;
        public Vector2 LabelOffset = new Vector2(10.5f, 1.5f);
        public float LineLength = 0.5f;

        private static Material _lineMaterial;
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private Camera _camera;
        private ObjectPool _labelsPool;
        private Vector3 _position;
        private Vector3 _up;

        public enum GridModes
        {
            Disabled,
            EnabledHorizontal,
            EnabledOrientedGrid,
        }

        public void SetPlane(SlamInfinitePlane plane)
        {
            _position = plane.Offset;
            _up = plane.Normal;
        }

        public void SetMode(int modeId)
        {
            Mode = (GridModes) modeId;
        }

        private void Awake()
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            _lineMaterial.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            _lineMaterial.SetInt(Cull, (int) UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            _lineMaterial.SetInt(ZWrite, 0);
            _camera = Camera.main;
            _labelsPool = new ObjectPool(LabelPrefab, transform);
        }

        // TODO: rewrite to Graphics.ProceduralDraw
        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            if (Mode == GridModes.Disabled)
            {
                _labelsPool.DespawnAllActiveObjects();
                return;
            }

            // Apply the line material
            _lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            if (Mode == GridModes.EnabledOrientedGrid)
            {
                transform.position = _position;
                transform.up = _up;
                GL.MultMatrix(transform.localToWorldMatrix);
            }
            else
            {
                transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            // Draw lines
            GL.Begin(GL.LINES);
            GL.Color(Color);
            var pos = transform.worldToLocalMatrix * _camera.transform.position;
            var offset = new Vector2(Mathf.Round(pos.x / Spacing) * Spacing,
                                     Mathf.Round(pos.z / Spacing) * Spacing);

            for (float i = -Length; i < Length; i += Spacing)
            {
                DrawGrid(offset, i);
            }

            for (float i = -DetailsVisibilityDistance; i < DetailsVisibilityDistance; i += Spacing)
            {
                for (float j = -DetailsVisibilityDistance; j < DetailsVisibilityDistance; j += Spacing)
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

        private void TryDrawLabels(Vector2 offset)
        {
            _labelsPool.DespawnAllActiveObjects();
            for (float i = -DetailsVisibilityDistance; i < DetailsVisibilityDistance; i += Spacing)
            {
                for (float j = -DetailsVisibilityDistance; j < DetailsVisibilityDistance; j += Spacing)
                {
                    var visible = IsGridPointVisible(new Vector2(i, j) + offset, DetailsVisibilityDistance);
                    if (!visible) continue;
                    var go = _labelsPool.Spawn();
                    if (Mode == GridModes.EnabledOrientedGrid)
                    {
                        go.transform.localPosition = new Vector3(offset.x + i + LabelOffset.x, 
                                                                 0, 
                                                                 offset.y + j - LabelOffset.y);
                        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        go.transform.position = new Vector3(offset.x + i  + LabelOffset.x, 
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
            if (Mode == GridModes.EnabledOrientedGrid)
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
            for (float i = -Spacing; i < Spacing; i += Spacing / 10)
            {
                GL.Vertex3(offset.x + i, 0, offset.y + LineLength);
                GL.Vertex3(offset.x + i, 0, offset.y - LineLength);
                GL.Vertex3(offset.x + LineLength, 0, offset.y + i);
                GL.Vertex3(offset.x - LineLength, 0, offset.y + i);
            }
        }

        private void DrawGrid(Vector2 offset, float i)
        {
            GL.Color(Color * (1 - Mathf.Abs(i / Length)));
            GL.Vertex3(offset.x, 0, offset.y + i);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x - Length, 0, offset.y + i);

            GL.Color(Color * (1 - Mathf.Abs(i / Length)));
            GL.Vertex3(offset.x, 0, offset.y + i);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x + Length, 0, offset.y + i);

            GL.Color(Color * (1 - Mathf.Abs(i / Length)));
            GL.Vertex3(offset.x + i, 0, offset.y);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x + i, 0, offset.y - Length);

            GL.Color(Color * (1 - Mathf.Abs(i / Length)));
            GL.Vertex3(offset.x + i, 0, offset.y);
            GL.Color(Color.clear);
            GL.Vertex3(offset.x + i, 0, offset.y + Length);
        }
    }
}