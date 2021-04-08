using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Renderers;
using Elektronik.UI.Windows;

namespace Elektronik.Protobuf.Data
{
    public abstract class ImagePresenter<T> : ISourceTree, IRendersToWindow
    {
        protected ImagePresenter(string displayName)
        {
            DisplayName = displayName;
        }
        
        #region ISourceTree

        public string DisplayName { get; set; }
        public IEnumerable<ISourceTree> Children => new ISourceTree[0];

        #endregion

        public void Clear()
        {
            Renderer.Clear();
        }

        public abstract void Present(T data);

        public void SetRenderer(object dataRenderer)
        {
            if (dataRenderer is WindowsManager factory)
            {
                factory.CreateWindow<ImageRenderer>(DisplayName, (renderer, window) =>
                {
                    Renderer = renderer;
                    Window = window;
                });
            }
        }

        #region IRendersToWindow

        public Window Window { get; private set; }
        public string Title { get; set; }

        #endregion

        #region Protected

        protected IDataRenderer<byte[]> Renderer;

        #endregion

    }
}