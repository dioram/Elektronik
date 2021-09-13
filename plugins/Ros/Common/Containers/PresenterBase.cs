using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.Renderers;
using Elektronik.UI.Windows;

namespace Elektronik.RosPlugin.Common.Containers
{
    public abstract class PresenterBase<TMessage, TRenderer, TRendererType> 
            : IPresenter<TMessage>, ISourceTreeNode, IRendersToWindow
            where TRenderer : class, IDataRenderer<TRendererType>
    {
        protected PresenterBase(string displayName)
        {
            DisplayName = displayName;
        }

        #region IPresenter

        public TMessage Current { get; private set; }

        public void Present(TMessage data)
        {
            Current = data;
            if (Renderer is null || !Renderer.IsShowing) return;
            Task.Run(() => Renderer.Render(ToRenderType(data)));
        }

        #endregion

        #region ISourceTreeNode

        public string DisplayName { get; set; }
        public IEnumerable<ISourceTreeNode> Children { get; } = Array.Empty<ISourceTreeNode>();

        public void Clear()
        {
            if (Renderer is not null) Renderer.Clear();
        }

        public void AddRenderer(ISourceRenderer renderer)
        {
            if (renderer is WindowsManager factory)
            {
                factory.CreateWindow<TRenderer>(Title, (r, window) =>
                {
                    Renderer = r;
                    Window = window;
                    SetRendererCallback();
                });
            }
        }

        public void RemoveRenderer(ISourceRenderer renderer)
        {
            if (Renderer != renderer) return;
            Renderer = null;
            Window = null;
        }

        #endregion

        #region IRendersToWindow

        public Window? Window { get; private set; }
        public string Title { get; set; }

        #endregion

        #region Protected

        protected TRenderer? Renderer;
        
        protected virtual void SetRendererCallback()
        {}

        protected abstract TRendererType ToRenderType(TMessage message);

        #endregion
    }
}