using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.VFX;

namespace Elektronik.Common.Clouds
{
    [RequireComponent(typeof(VisualEffect))]
    public class PointCloudRenderer : MonoBehaviour
    {
        public FastPointCloudV2 PointsCloud;
        public float PointSize = 0.1f;
        public VisualEffectAsset VFXAssetPrefab;

        private List<PointCloudBlock> _blocks = new List<PointCloudBlock>();
        private bool _needNewBlock;

        private void Start()
        {
            PointsCloud.PointsAdded += OnPointsAdded;
            PointsCloud.PointsUpdated += OnPointsUpdated;
            PointsCloud.PointsRemoved += OnPointsRemoved;
            PointsCloud.PointsCleared += OnPointsCleared;
            _needNewBlock = true;
        }

        private void Update()
        {
            if (_needNewBlock)
            {
                var go = new GameObject($"Point cloud block {_blocks.Count}");
                go.transform.SetParent(transform);
                var block = go.AddComponent<PointCloudBlock>();
                block.VFXAssetPrefab = VFXAssetPrefab;
                block.Updated = true;
                _blocks.Add(block);
                _needNewBlock = false;
                Debug.LogError(PointsCloud.Count);
            }
        }

        private void OnPointsAdded(IList<CloudPoint> points)
        {
            if (PointsCloud.Count > (_blocks.Count - 1) * PointCloudBlock.Capacity)
            {
                _needNewBlock = true;
            }

            foreach (var point in points)
            {
                int layer = point.idx / PointCloudBlock.Capacity;
                int inLayerId = point.idx % PointCloudBlock.Capacity;
                var posSizeColor = new Color(point.offset.x, point.offset.y, point.offset.z, PointSize);
                _blocks[layer].PixelColors[inLayerId] = point.color;
                _blocks[layer].PixelPosSize[inLayerId] = posSizeColor;
                _blocks[layer].CalculateBounds(point.offset);
                _blocks[layer].PointsCount++;
                _blocks[layer].Updated = true;
            }
        }

        private void OnPointsUpdated(IList<CloudPoint> points)
        {
            foreach (var point in points)
            {
                int layer = point.idx / PointCloudBlock.Capacity;
                int inLayerId = point.idx % PointCloudBlock.Capacity;
                var posSizeColor = new Color(point.offset.x, point.offset.y, point.offset.z, PointSize);
                _blocks[layer].PixelColors[inLayerId] = point.color;
                _blocks[layer].PixelColors[inLayerId] = posSizeColor;
                _blocks[layer].CalculateBounds(point.offset);
                _blocks[layer].Updated = true;
            }
        }

        private void OnPointsRemoved(IList<int> removedPointsIds)
        {
            foreach (var pointId in removedPointsIds)
            {
                int layer = pointId / PointCloudBlock.Capacity;
                int inLayerId = pointId % PointCloudBlock.Capacity;
                _blocks[layer].PixelColors[inLayerId] = new Color(0, 0, 0, 0);
                _blocks[layer].Updated = true;
            }
        }

        private void OnPointsCleared()
        {
            foreach (var block in _blocks)
            {
                Destroy(block);
            }
        }
    }
}