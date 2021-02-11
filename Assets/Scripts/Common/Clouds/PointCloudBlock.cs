using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Elektronik.Common.Clouds
{
    [RequireComponent(typeof(VisualEffect))]
    public class PointCloudBlock : MonoBehaviour
    {
        public const int Resolution = 1024;
        public const int Capacity = Resolution * Resolution;
        public VisualEffectAsset VFXAssetPrefab;
        public bool Updated;
        public Color[] PixelColors;
        public Color[] PixelPosSize;
        public int PointsCount;

        private VisualEffect _vfxRenderer;
        private Texture2D _texColor;
        private Texture2D _texPosSize;
        private Vector3 _minBound;
        private Vector3 _maxBound;

        public void CalculateBounds(Vector3 point)
        {
            _minBound = new Vector3(Mathf.Min(point.x, _minBound.x),
                                    Mathf.Min(point.y, _minBound.y),
                                    Mathf.Min(point.z, _minBound.z));
            _maxBound = new Vector3(Mathf.Max(point.x, _maxBound.x),
                                    Mathf.Max(point.y, _maxBound.y),
                                    Mathf.Max(point.z, _maxBound.z));
        }

        private void Start()
        {
            _vfxRenderer = GetComponent<VisualEffect>();
            _vfxRenderer.visualEffectAsset = VFXAssetPrefab;
            _texColor = new Texture2D(Resolution, Resolution, TextureFormat.RGBAFloat, false);
            _texPosSize = new Texture2D(Resolution, Resolution, TextureFormat.RGBAFloat, false);
            PixelColors = new Color[Capacity];
            PixelPosSize = new Color[Capacity];
        }

        private void Update()
        {
            if (!Updated) return;

            _texColor.SetPixels(PixelColors);
            _texPosSize.SetPixels(PixelPosSize);
            _texColor.Apply();
            _texPosSize.Apply();

            Vector3 center;
            Vector3 size;
            if (PointsCount == 0)
            {
                center = Vector3.zero;
                size = Vector3.one;
            }
            else
            {
                center = (_minBound + _maxBound) / 2;
                size = _maxBound - _minBound;
            }

            _vfxRenderer.Reinit();
            _vfxRenderer.SetUInt(Shader.PropertyToID("particleCount"), (uint) PointsCount);
            _vfxRenderer.SetTexture(Shader.PropertyToID("texColor"), _texColor);
            _vfxRenderer.SetTexture(Shader.PropertyToID("texPosScale"), _texPosSize);
            _vfxRenderer.SetUInt(Shader.PropertyToID("resolution"), (uint) Resolution);
            _vfxRenderer.SetVector3(Shader.PropertyToID("boundsCenter"), center);
            _vfxRenderer.SetVector3(Shader.PropertyToID("boundsSize"), size);

            Updated = false;
        }
    }
}