using System;
using System.Collections.Generic;
using Elektronik.Data;
using Elektronik.Renderers;
using Elektronik.UI.Windows;

namespace Elektronik.Protobuf.Data
{
    public abstract class ImagePresenter<T> : ISourceTreeNode, IRendersToWindow
    {
        protected ImagePresenter(string displayName)
        {
            DisplayName = displayName;
        }
        
        #region ISourceTreeNode

        public string DisplayName { get; set; }
        
        public IEnumerable<ISourceTreeNode> Children => Array.Empty<ISourceTreeNode>();

        #endregion

        public void Clear()
        {
            Renderer?.Clear();
        }

        public abstract void Present(T data);

        public void AddRenderer(ISourceRenderer dataRenderer)
        {
            if (dataRenderer is WindowsManager factory)
            {
                factory.CreateWindow<ImageRenderer>(DisplayName, (renderer, window) =>
                {
                    Renderer = renderer;
                    Window = window;
                });
            }

            if (dataRenderer is IDataRenderer<byte[]> renderer)
            {
                Renderer = renderer;
            }
        }

        public void RemoveRenderer(ISourceRenderer renderer)
        {
            if (Renderer != renderer) return;
            Renderer = null;
            Window = null;
        }

        #region IRendersToWindow

        public Window? Window { get; private set; }
        public string Title { get; set; }

        #endregion

        #region Protected

        protected IDataRenderer<byte[]>? Renderer;

        #endregion

    }
}