﻿using System;
using System.Collections.Generic;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.Windows;
using Elektronik.DataSources;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.UI.Windows;

namespace Elektronik.Protobuf.Data
{
    public class ImagePresenter : ISourceTreeNode, IRendersToWindow
    {
        public ImagePresenter(string displayName)
        {
            DisplayName = displayName;
        }
        
        #region ISourceTreeNode

        public ISourceTreeNode? TakeSnapshot() => null;

        public string DisplayName { get; set; }
        
        public IEnumerable<ISourceTreeNode> Children => Array.Empty<ISourceTreeNode>();

        #endregion

        public void Clear()
        {
            Renderer?.Clear();
        }

        public void Present(byte[] data)
        {
            Renderer?.Render(data);
        }

        public void AddConsumer(IDataConsumer consumer)
        {
            switch (consumer)
            {
                case WindowsManager factory:
                    factory.CreateWindow<ImageRenderer>(DisplayName, (renderer, window) =>
                    {
                        Renderer = renderer;
                        Window = window;
                    });
                    break;
                case IDataRenderer<byte[]> renderer:
                    Renderer = renderer;
                    break;
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            if (Renderer != consumer) return;
            Renderer = null;
            Window = null;
        }

        #region IRendersToWindow

        public Window? Window { get; private set; }
        public string? Title { get; set; }

        #endregion

        #region Protected

        protected IDataRenderer<byte[]>? Renderer;

        #endregion

    }
}