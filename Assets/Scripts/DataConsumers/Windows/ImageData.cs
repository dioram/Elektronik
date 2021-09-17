using UnityEngine;

namespace Elektronik.DataConsumers.Windows
{
    public struct ImageData
    {
        public int Width;
        public int Height;
        public TextureFormat Encoding;
        public byte[] Data;
        public bool IsSupported;
    }
}