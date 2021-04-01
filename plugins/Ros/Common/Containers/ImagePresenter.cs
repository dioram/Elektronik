using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.RosMessages;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class ImagePresenter
            : PresenterBase<
                ImagePresenter.ImageData,
                ImageRenderer,
                (int width, int height, byte[] array, TextureFormat format)>
    {
        public class ImageData
        {
            public readonly int Width;
            public readonly int Height;
            public readonly string Encoding;
            public readonly byte[] Data;

            public ImageData(int width, int height, string encoding, byte[] data)
            {
                Width = width;
                Height = height;
                Encoding = encoding;
                Data = data;
            }
        }

        public ImagePresenter([NotNull] string displayName) : base(displayName)
        {
        }

        #region PresenterBase

        protected override void SetRendererCallback()
        {
            base.SetRendererCallback();
            if (Renderer is not null) Renderer.FlipVertically = true;
        }

        protected override (int width, int height, byte[] array, TextureFormat format) ToRenderType(ImageData message)
        {
            return (message.Width, message.Height, message.Data,
                    RosMessageConvertExtender.GetTextureFormat(message.Encoding));
        }

        #endregion
    }
}