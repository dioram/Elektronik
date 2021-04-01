using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.UI.Windows;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class ImagePresenter : ISourceTree, IRendersToWindow
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

        public ImagePresenter(string displayName)
        {
            DisplayName = displayName;
        }

        public void Present(ImageData data)
        {
            Current = data;
            if (_renderer is null || !_renderer.IsShowing) return;
            _renderer.Render((data.Width, data.Height, data.Data,
                              RosMessageConvertExtender.GetTextureFormat(data.Encoding)));
        }

        public ImageData Current;

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; } = new ISourceTree[0];

        public bool IsActive { get; set; }

        public void Clear()
        {
            if (_renderer is not null) _renderer.Clear();
        }

        public void SetRenderer(object renderer)
        {
            if (renderer is WindowsFactory factory)
            {
                factory.GetNewDataRenderer<ImageRenderer>(DisplayName, (imageRenderer, window) =>
                {
                    _renderer = imageRenderer;
                    _renderer.FlipVertically = true;
                    Window = window;
                });
            }
        }

        #endregion

        #region IRendersToWIndow

        public Window Window { get; private set; }

        #endregion

        #region Private

        private ImageRenderer? _renderer;

        #endregion
    }
}