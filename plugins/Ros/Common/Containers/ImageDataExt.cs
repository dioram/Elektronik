using System;
using Elektronik.DataConsumers.Windows;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;

namespace Elektronik.RosPlugin.Common.Containers
{
    public static class ImageDataExt
    {
        public static ImageData? FromImageMessage(Image image)
        {
            try
            {
                return new ImageData((int)image.width, (int)image.height, GetTextureFormat(image.encoding), image.data);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
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