using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Cameras
{
    public class Grid : MonoBehaviour
    {
        public float Spacing = 10f;
        public float Length = 1000f;
        public Color Color = Color.white;
        public GridModes Mode;

        private static Material _lineMaterial;
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        public enum GridModes
        {
            Disabled,
            EnabledHorizontal,
            EnabledOrientedGrid,
        }

        public void SetPlane(SlamInfinitePlane plane)
        {
            transform.position = plane.Offset;
            transform.up = plane.Normal;
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
        }

        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            if (Mode == GridModes.Disabled) return;

            // Apply the line material
            _lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            if (Mode == GridModes.EnabledOrientedGrid)
            {
                GL.MultMatrix(transform.localToWorldMatrix);
            }

            // Draw lines
            GL.Begin(GL.LINES);
            GL.Color(Color);

            for (float i = -Length; i < Length; i += Spacing)
            {
                GL.Vertex3(-Length, 0, i);
                GL.Vertex3(Length, 0, i);
                GL.Vertex3(i, 0, -Length);
                GL.Vertex3(i, 0, Length);
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}