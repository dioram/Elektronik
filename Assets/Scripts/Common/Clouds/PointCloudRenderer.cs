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

        private static readonly int[] TextureResolutions =
        {
                2 * 2,
                4 * 4,
                8 * 8,
                16 * 16,
                32 * 32,
                64 * 64,
                128 * 128,
                256 * 256,
                512 * 512,
                1024 * 1024,
                2048 * 2048,
                4096 * 4096
        };

        private int _currentResolution;
        private int _currentAmount;
        private int _currentReserved;
        private readonly Dictionary<int, int> _pointsIds = new Dictionary<int, int>();
        private bool _dontReinit;
        private bool _resolutionChanged;
        private Vector3 minBound;
        private Vector3 maxBound;


        private void Start()
        {
            m_renderer = GetComponent<VisualEffect>();
            PointsCloud.PointsAdded += OnPointsAdded;
            PointsCloud.PointsUpdated += OnPointsUpdated;
            PointsCloud.PointsRemoved += OnPointsRemoved;
            PointsCloud.PointsCleared += OnPointsCleared;
        }

        private void Update()
        {
            if (toUpdate) {
                toUpdate = false;
                
                int texSide = (int) Mathf.Sqrt(_currentResolution);
                if (_resolutionChanged)
                {
                    texColor = new Texture2D(texSide, texSide, TextureFormat.RGBAFloat, false);
                    texPosScale = new Texture2D(texSide, texSide, TextureFormat.RGBAFloat, false);
                    _resolutionChanged = false;
                }

                texColor.SetPixels(pixelColors);
                texPosScale.SetPixels(pixelPosSize);
                texColor.Apply();
                texPosScale.Apply();

                Vector3 center = _currentAmount == 0 ? Vector3.zero : (minBound + maxBound) / 2;
                Vector3 size = _currentAmount == 0 ? Vector3.one : maxBound - minBound;

                m_renderer.Reinit();
                m_renderer.SetUInt(Shader.PropertyToID("particleCount"), (uint) _currentReserved);
                m_renderer.SetTexture(Shader.PropertyToID("texColor"), texColor);
                m_renderer.SetTexture(Shader.PropertyToID("texPosScale"), texPosScale);
                m_renderer.SetUInt(Shader.PropertyToID("resolution"), (uint) texSide);
                m_renderer.SetVector3(Shader.PropertyToID("boundsCenter"), center);
                m_renderer.SetVector3(Shader.PropertyToID("boundsSize"), size);
            }
        }

        
        private void OnPointsAdded(IList<CloudPoint> points)
        {
            if (_currentReserved + points.Count > _currentResolution)
            {
                ChangeResolution();
                return;
            }

            for (int i = 0; i < points.Count; i++)
            {
                pixelPosSize[_currentAmount + i] = new Color(points[i].offset.x,
                        points[i].offset.y,
                        points[i].offset.z,
                        PointSize);
                pixelColors[_currentAmount + i] = points[i].color;
                _pointsIds[points[i].idx] = _currentAmount + i;
                CalculateBounds(points[i].offset);
            }

            _currentAmount += points.Count;
            _currentReserved += points.Count;

            toUpdate = true;
        }

        private void OnPointsUpdated(IList<CloudPoint> points)
        {
            foreach (var point in points)
            {
                pixelPosSize[_pointsIds[point.idx]] =
                        new Color(point.offset.x, point.offset.y, point.offset.z, PointSize);
                pixelColors[_pointsIds[point.idx]] = point.color;
                CalculateBounds(point.offset);
            }

            _dontReinit = true;
            toUpdate = true;
        }

        private void OnPointsRemoved(IList<int> removedPointsIds)
        {
            if (_currentAmount - removedPointsIds.Count < LowerResolution())
            {
                ChangeResolution();
                return;
            }

            _currentAmount -= _pointsIds.Count;
            foreach (var pointId in removedPointsIds)
            {
                pixelPosSize[_pointsIds[pointId]] = new Color(0, 0, 0, 0);
                _pointsIds.Remove(pointId);
            }

            _dontReinit = true;
            toUpdate = true;
        }

        private void OnPointsCleared()
        {
            _pointsIds.Clear();
            ChangeResolution();
            toUpdate = true;
        }

        private void ChangeResolution()
        {
            var points = PointsCloud.GetAll().ToList();
            _currentResolution = TextureResolutions.First(r => r > points.Count);
            pixelColors = new Color[_currentResolution];
            pixelPosSize = new Color[_currentResolution];
            _resolutionChanged = true;
            _currentAmount = points.Count;
            _currentReserved = points.Count;
            minBound = Vector3.positiveInfinity;
            maxBound = Vector3.negativeInfinity;

            for (int i = 0; i < points.Count; i++)
            {
                pixelPosSize[i] = new Color(points[i].offset.x, points[i].offset.y, points[i].offset.z, PointSize);
                pixelColors[i] = points[i].color;
                _pointsIds[points[i].idx] = i;
                CalculateBounds(points[i].offset);
            }

            toUpdate = true;
        }

        private void CalculateBounds(Vector3 point)
        {
            minBound = new Vector3(Mathf.Min(point.x, minBound.x),
                    Mathf.Min(point.y, minBound.y),
                    Mathf.Min(point.z, minBound.z));
            maxBound = new Vector3(Mathf.Max(point.x, maxBound.x),
                    Mathf.Max(point.y, maxBound.y),
                    Mathf.Max(point.z, maxBound.z));
        }

        private int LowerResolution()
        {
            int i = 0;
            foreach (var resolution in TextureResolutions)
            {
                if (resolution >= _currentResolution) break;
                i = resolution;
            }

            return i;
        }
    }
}