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
        public Shader PointCloudShader;

        private List<PointCloudBlock> _blocks = new List<PointCloudBlock>();
        private bool _needNewBlock;
        private bool _toClear;

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
                block.PointCloudShader = PointCloudShader;
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
                _blocks[layer].Points[inLayerId] = new CloudPointV2(point);
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
                _blocks[layer].Points[inLayerId] = new CloudPointV2(point);
                _blocks[layer].Updated = true;
            }
        }

        private void OnPointsRemoved(IList<int> removedPointsIds)
        {
            foreach (var pointId in removedPointsIds)
            {
                int layer = pointId / PointCloudBlock.Capacity;
                int inLayerId = pointId % PointCloudBlock.Capacity;
                _blocks[layer].Points[inLayerId] = CloudPointV2.Empty();
                _blocks[layer].Updated = true;
            }
        }

        private void OnPointsCleared()
        {
            foreach (var block in _blocks)
            {
                block.ToClear = true;
            }
        }
    }
}