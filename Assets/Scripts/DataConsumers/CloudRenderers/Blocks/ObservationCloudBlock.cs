using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationCloudBlock : CloudBlock
    {
        // ReSharper disable once InconsistentNaming
        public struct Matrix3x3
        {
#pragma warning disable 414
            private float _m00, _m01, _m02, _m10, _m11, _m12, _m20, _m21, _m22;
#pragma warning restore 414
            
            public static implicit operator Matrix3x3(Matrix4x4 m)
            {
                return new Matrix3x3
                {
                    _m00 = m.m00,
                    _m01 = m.m01,
                    _m02 = m.m02,
                    _m10 = m.m10,
                    _m11 = m.m11,
                    _m12 = m.m12,
                    _m20 = m.m20,
                    _m21 = m.m21,
                    _m22 = m.m22,
                };
            }
        }
        
        public Matrix3x3[] Rotations;
        public Vector3[] Positions;
        public Color[] Colors;

        protected override void Init()
        {
            Positions = Enumerable.Repeat(default(Vector3), Capacity).ToArray();
            Rotations = Enumerable.Repeat(default(Matrix3x3), Capacity).ToArray();
            Colors = Enumerable.Repeat(default(Color), Capacity).ToArray();
            _rotationsBuffer = new ComputeBuffer(Rotations.Length, sizeof(float) * 9);
            _positionsBuffer = new ComputeBuffer(Positions.Length, sizeof(float) * 3);
            _colorsBuffer = new ComputeBuffer(Colors.Length, sizeof(float) * 4);
        }

        protected override void SendData(Material renderMaterial)
        {
            renderMaterial.SetBuffer(_rotationsBufferShaderProp, _rotationsBuffer);
            renderMaterial.SetBuffer(_positionsBufferShaderProp, _positionsBuffer);
            renderMaterial.SetBuffer(_colorsBufferShaderProp, _colorsBuffer);
        }

        protected override void OnUpdated()
        {
            _rotationsBuffer.SetData(Rotations);
            _positionsBuffer.SetData(Positions);
            _colorsBuffer.SetData(Colors);
        }

        protected override void Draw()
        {
            Graphics.DrawProceduralNow(MeshTopology.Points, _rotationsBuffer.count);
        }

        protected override void ReleaseBuffers()
        {
            _rotationsBuffer.Release();
            _positionsBuffer.Release();
            _colorsBuffer.Release();
        }

        #region Private
        
        private readonly int _rotationsBufferShaderProp = Shader.PropertyToID("_RotationsBuffer");
        private readonly int _positionsBufferShaderProp = Shader.PropertyToID("_PositionsBuffer");
        private readonly int _colorsBufferShaderProp = Shader.PropertyToID("_ColorsBuffer");
        private ComputeBuffer _rotationsBuffer;
        private ComputeBuffer _positionsBuffer;
        private ComputeBuffer _colorsBuffer;

        #endregion
    }
}