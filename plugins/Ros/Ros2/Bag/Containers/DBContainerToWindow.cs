using System.Collections.Generic;
using Elektronik.Renderers;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using Elektronik.UI.Windows;
using SQLite;
using Message = RosSharp.RosBridgeClient.Message;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public abstract class DBContainerToWindow<TMessage, TRender, TRenderType>
            : DBContainer<TMessage, TRenderType>, IRendersToWindow
            where TMessage : Message
            where TRender : class, IDataRenderer<TRenderType>
    {
        public DBContainerToWindow(string displayName, List<SQLiteConnection> dbModels, Topic topic,
                                   List<long> actualTimestamps)
                : base(displayName, dbModels, topic, actualTimestamps)
        {
        }

        #region DBContainer

        public override void Clear()
        {
            if (Renderer is not null) Renderer.Clear();
        }

        public override void AddRenderer(ISourceRenderer renderer)
        {
            if (renderer is WindowsManager factory)
            {
                factory.CreateWindow<TRender>(Title, (r, window) =>
                {
                    Renderer = r;
                    Window = window;
                    SetRendererCallback();
                });
            }
        }

        public override void RemoveRenderer(ISourceRenderer renderer)
        {
            if (Renderer != renderer) return;
            Renderer = null;
            Window = null;
        }

        protected override void SetData()
        {
            base.SetData();
            if (Renderer is not null && Current is not null)
            {
                Renderer.Render(Current);
            }
        }

        #endregion

        #region IRendersToWindow

        public Window? Window { get; private set; }
        public string Title { get; set; }

        #endregion

        #region Protected

        protected TRender? Renderer;

        protected virtual void SetRendererCallback()
        {
        }

        #endregion
    }
}