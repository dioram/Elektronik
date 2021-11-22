using UnityEngine;

namespace Elektronik.UI
{
    /// <summary> This component draws coordinate axes in local system of game object. </summary>
    internal class XYZAxis : MonoBehaviour
    {
        /// <summary> Should backward directions of axes be rendered. </summary>
        [Tooltip("Should backward directions of axes be rendered.")]
        public bool DrawBackwardAxes;
        
        /// <summary> Length of axis line. </summary>
        [Tooltip("Length of axis line.")]
        public float LengthOfAxis = 0.5f;

        #region Unity events

        private void Awake()
        {
            if (_lineMaterial) return;
            
            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _lineMaterial.SetInt(_srcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt(_dstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt(_cull, (int) UnityEngine.Rendering.CullMode.Off);
        }

        private void OnRenderObject()
        {
            // TODO: It is not really necessary but can be rewritten to Graphics.ProceduralDraw
            _lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(LengthOfAxis, 0.0f, 0.0f);
            GL.Color(Color.green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0.0f, LengthOfAxis, 0.0f);
            GL.Color(Color.blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0.0f, 0.0f, LengthOfAxis);

            if (DrawBackwardAxes)
            {
                GL.Color(Color.red / 2);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(-LengthOfAxis, 0.0f, 0.0f);
                GL.Color(Color.green / 2);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0.0f, -LengthOfAxis, 0.0f);
                GL.Color(Color.blue / 2);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0.0f, 0.0f, -LengthOfAxis);
            }
            
            GL.End();
            GL.PopMatrix();
        }

        #endregion

        #region Private

        private Material _lineMaterial;
        private readonly int _srcBlend = Shader.PropertyToID("_SrcBlend");
        private readonly int _dstBlend = Shader.PropertyToID("_DstBlend");
        private readonly int _cull = Shader.PropertyToID("_Cull");

        #endregion
    }
}