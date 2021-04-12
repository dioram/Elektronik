using System;
using Elektronik.Renderers;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;

namespace Elektronik.RosPlugin.Common.Containers
{
    public static class ImageDataExt
    {
        public static ImageData FromImageMessage(Image image)
        {
            try
            {
                return new ImageData
                {
                    Width = (int) image.width,
                    Height = (int) image.height,
                    Encoding = GetTextureFormat(image.encoding),
                    Data = image.data,
                    IsSupported = true,
                };
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ImageData{IsSupported = false};
            }
        }

        public static TextureFormat GetTextureFormat(string encoding) =>
                encoding switch
                {
                    "rgb8" => TextureFormat.RGB24,
                    "rgba8" => TextureFormat.RGBA32,
                    "rgb16" => TextureFormat.RGB48,
                    "rgba16" => TextureFormat.RGBA64,
                    "bgra8" => TextureFormat.BGRA32,
                    "mono8" => TextureFormat.Alpha8,
                    "mono16" => TextureFormat.R16,
                    _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding,
                                                               "This type of encoding is not supported.")
                };
    }
}