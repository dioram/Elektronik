using UnityEngine;

namespace Elektronik.Clouds
{
    public class XYZAxis : MonoBehaviour
    {
        public float LengthOfAxis = 0.5f;
        private static Material _lineMaterial;
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        static void CreateLineMaterial()
        {
            if (!_lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                _lineMaterial = new Material(shader);
                _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                _lineMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _lineMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                _lineMaterial.SetInt(Cull, (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                _lineMaterial.SetInt(ZWrite, 0);
            }
        }

        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            CreateLineMaterial();
            // Apply the line material
            _lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);
            //Draw X axis
            GL.Color(Color.red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(LengthOfAxis, 0.0f, 0.0f);
            //Draw Y axis
            GL.Color(Color.green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0.0f, -LengthOfAxis, 0.0f);
            //Draw Z axis
            GL.Color(Color.blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0.0f, 0.0f, LengthOfAxis);
            GL.End();
            GL.PopMatrix();
        }
    }
}