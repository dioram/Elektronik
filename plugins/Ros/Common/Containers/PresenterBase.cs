using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Renderers;
using Elektronik.UI.Windows;

namespace Elektronik.RosPlugin.Common.Containers
{
    public abstract class PresenterBase<TMessage, TRenderer, TRendererType> 
            : IPresenter<TMessage>, ISourceTree, IRendersToWindow
            where TRenderer : IDataRenderer<TRendererType>
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
            Renderer.Render(ToRenderType(data));
        }

        #endregion

        #region ISourceTree

        public string DisplayName { get; set; }
        public IEnumerable<ISourceTree> Children { get; } = new ISourceTree[0];
        public bool IsActive { get; set; }

        public void Clear()
        {
            if (Renderer is not null) Renderer.Clear();
        }

        public void SetRenderer(object renderer)
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

        #endregion

        #region IRendersToWindow

        public Window Window { get; private set; }
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