using Elektronik.Common.Extensions;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public struct CloudPointV2
    {
        public const int Size = sizeof(float) * 4;
            
        public Vector3 Position;
        public uint Color;

        public static CloudPointV2 Empty()
        {
            return new CloudPointV2
            {
                    Position = Vector3.zero,
                    Color = 0,
            };
        }

        public CloudPointV2(CloudPoint cp)
        {
            Position = cp.offset;
            Color = EncodeColor(cp.color);
        }
            
        static uint EncodeColor(Color c)
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