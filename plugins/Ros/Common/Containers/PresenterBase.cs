using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.Windows;
using Elektronik.DataSources;
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

        public TMessage? Current { get; private set; }

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

        public void AddConsumer(IDataConsumer consumer)
        {
            if (consumer is WindowsManager factory)
            {
                factory.CreateWindow<TRenderer>(Title, (r, window) =>
                {
                    Renderer = r;
                    Window = window;
                    SetRendererCallback();
                });
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            if (Renderer != consumer) return;
            Renderer = null;
            Window = null;
        }

        #endregion

        #region IRendersToWindow

        public Window? Window { get; private set; }
        public string? Title { get; set; }

        #endregion

        #region Protected

        protected TRenderer? Renderer;
        
        protected virtual void SetRendererCallback()
        {}

        protected abstract TRendererType ToRenderType(TMessage message);

        #endregion
    }
}