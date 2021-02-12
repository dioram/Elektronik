using System;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class PointCloudBlock : MonoBehaviour
    {
        public const int Capacity = 1024 * 1024;
        public Shader PointCloudShader;
        public bool Updated;
        public CloudPointV2[] Points;
        public int PointsCount;
        public bool ToClear;
        
        public float PointSize = 1f;
        private static readonly int PointBuffer = Shader.PropertyToID("_PointBuffer");
        private static readonly int Size = Shader.PropertyToID("_PointSize");
        private Material _renderMaterial;
        private ComputeBuffer _pointsBuffer;

        private void Awake()
        {
            Points = Enumerable.Range(0, Capacity).Select(_ => CloudPointV2.Empty()).ToArray();
            _pointsBuffer = new ComputeBuffer(Points.Length, CloudPointV2.Size);
        }

        private void Start()
        {
            _renderMaterial = new Material(PointCloudShader);
            _renderMaterial.hideFlags = HideFlags.DontSave;
            _renderMaterial.EnableKeyword("_COMPUTE_BUFFER");
        }

        private void Update()
        {
            if (ToClear)
            {
                Points = Enumerable.Range(0, Capacity).Select(_ => CloudPointV2.Empty()).ToArray();
                ToClear = false;
                Updated = true;
            }
            
            if (!Updated) return;
            
            _pointsBuffer.SetData(Points);

            Updated = false;
        }

        private void OnRenderObject()
        {
            _renderMaterial.SetPass(0);
            _renderMaterial.SetBuffer(PointBuffer, _pointsBuffer);
            _renderMaterial.SetFloat(Size, PointSize);
            Graphics.DrawProceduralNow(MeshTopology.Points, _pointsBuffer.count, 1);
        }
    }
}