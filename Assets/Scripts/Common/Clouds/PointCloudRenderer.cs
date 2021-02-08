using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Elektronik.Common.Clouds
{
    [RequireComponent(typeof(VisualEffect))]
    public class PointCloudRenderer : MonoBehaviour
    {
        public FastPointCloudV2 PointsCloud;
        public float PointSize = 0.1f;

        private bool toUpdate;
        private Texture2D texColor;
        private Texture2D texPosScale;
        private int particleCount;
        private int textureResolution = 1024;
        private Vector3 center;
        private Vector3 size;
        private VisualEffect m_renderer;
        private Color[] pixelColors;
        private Color[] pixelPosSize;

        private void Start()
        {
            m_renderer = GetComponent<VisualEffect>();
            PointsCloud.CloudUpdated += OnPointsCloudUpdated;
            texColor = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBAFloat, false);
            texPosScale = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBAFloat, false);
        }

        private void Update()
        {
            if (toUpdate) {
                toUpdate = false;
                
                texColor.SetPixels(pixelColors);
                texPosScale.SetPixels(pixelPosSize);
                texColor.Apply();
                texPosScale.Apply();
                
                m_renderer.Reinit();
                m_renderer.SetUInt(Shader.PropertyToID("particleCount"), (uint)particleCount);
                m_renderer.SetTexture(Shader.PropertyToID("texColor"), texColor);
                m_renderer.SetTexture(Shader.PropertyToID("texPosScale"), texPosScale);
                m_renderer.SetUInt(Shader.PropertyToID("resolution"), (uint)textureResolution);
                m_renderer.SetVector3(Shader.PropertyToID("boundsCenter"), center);
                m_renderer.SetVector3(Shader.PropertyToID("boundsSize"), size);
            }
        }

        private void OnPointsCloudUpdated()
        {
            var points = PointsCloud.GetAll();
            var positions = points.Select(p => p.offset);
            var encodedPositions = positions.Select(p => new Color(p.x, p.y, p.z, PointSize)).ToArray();
            var colors = points.Select(p => p.color).ToArray();
            particleCount = colors.Length;
            
            pixelColors = new Color[textureResolution * textureResolution];
            colors.CopyTo(pixelColors, 0);
            pixelPosSize = new Color[textureResolution * textureResolution];
            encodedPositions.CopyTo(pixelPosSize, 0);
            
            CalculateBounds(positions);
            toUpdate = true;
        }
        
        void CalculateBounds(IEnumerable<Vector3> positions)
        {
            if (!positions.Any())
            {
                center = Vector3.zero;
                size = Vector3.one * 5000f;
                return;
            }
        
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            foreach (var position in positions)
            {
                min = new Vector3(Mathf.Min(min.x, position.x), Mathf.Min(min.y, position.y), Mathf.Min(min.z, position.z));
                max = new Vector3(Mathf.Max(max.x, position.x), Mathf.Max(max.y, position.y), Mathf.Max(max.z, position.z));
            }

            center = (min + max) / 2;
            size = (max - min);
        }
    }
}