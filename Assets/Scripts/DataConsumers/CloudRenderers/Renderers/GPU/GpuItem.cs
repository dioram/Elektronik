using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Struct for sending point data to GPU. </summary>
    internal struct GpuItem
    {
        /// <summary> Size of this struct in bytes. </summary>
        public const int Size = sizeof(float) * 4;

        /// <summary> Position of point. </summary>
        public Vector3 Position;

        /// <summary> Encoded color of point. </summary>
        public uint Color;

        /// <summary> Constructs GPU item from <see cref="SlamPoint"/>. </summary>
        /// <param name="point"></param>
        public GpuItem(SlamPoint point)
        {
            Position = point.Position;
            Color = EncodeColor(point.Color);
        }

        /// <summary> Constructs GPU item from its position and color. </summary>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        public GpuItem(Vector3 offset, Color color)
        {
            Position = offset;
            Color = EncodeColor(color);
        }

        private static uint EncodeColor(Color c)
        {
            const float kMaxBrightness = 16;

            var y = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
            y = Mathf.Clamp(Mathf.Ceil(y * 255 / kMaxBrightness), 1, 255);

            var rgb = new Vector3(c.r, c.g, c.b);
            rgb *= 255 * 255 / (y * kMaxBrightness);

            return ((uint)rgb.x      ) |
                   ((uint)rgb.y <<  8) |
                   ((uint)rgb.z << 16) |
                   ((uint)y     << 24);
        }
    }
}