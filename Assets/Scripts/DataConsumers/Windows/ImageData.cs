using UnityEngine;

namespace Elektronik.DataConsumers.Windows
{
    /// <summary> This struct contains all data needed by elektronik for rendering image. </summary>
    public readonly struct ImageData
    {
        /// <summary> Image width. </summary>
        public readonly int Width;
        
        /// <summary> Image height. </summary>
        public readonly int Height;
        
        /// <summary> Image encoding. </summary>
        public readonly TextureFormat Encoding;
        
        /// <summary> Byte array of image data. </summary>
        public readonly byte[] Data;

        /// <summary> Marks that image need to flipped vertically. </summary>
        public readonly bool IsFlippedVertically;

        public ImageData(int width, int height, TextureFormat encoding, byte[] data, bool isFlippedVertically)
        {
            Width = width;
            Height = height;
            Encoding = encoding;
            Data = data;
            IsFlippedVertically = isFlippedVertically;
        }
    }
}