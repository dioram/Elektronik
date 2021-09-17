using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.UI.Windows;

namespace Elektronik.RosPlugin.Common.Containers
{
    public abstract class RosContainerTree : VirtualSource, IDisposable
    {
        public List<(string Name, string Type)> ActualTopics = new ();
        public readonly Dictionary<string, ISourceTreeNode> RealChildren = new();

        public RosContainerTree(string displayName) : base(displayName)
        {
        }

        public virtual void Dispose()
        {
            Clear();
            ActualTopics.Clear();
            RealChildren.Clear();
            ChildrenList.Clear();
        }

        public override void AddConsumer(IDataConsumer consumer)
        {
            _consumers.Add(consumer);
            base.AddConsumer(consumer);
        }

        public override void RemoveConsumer(IDataConsumer consumer)
        {
            _consumers.Remove(consumer);
            base.RemoveConsumer(consumer);
        }

        #region Protected

        protected abstract ISourceTreeNode CreateContainer(string topicName, string topicType);

        protected void RebuildTree()
        {
            BuildTree();
            Squeeze();
        }

        #endregion

        #region Private

        private readonly List<IDataConsumer> _consumers = new ();

        private void BuildTree()
        {
            ChildrenList.Clear();

            foreach (var topic in ActualTopics)
            {
                var path = topic.Name.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                VirtualSource parent = this;
                for (int i = 0; i < path.Length - 1; i++)
                {
                    if (parent.Children.FirstOrDefault(c => c.DisplayName == path[i]) is VirtualSource container)
                    {
                        parent = container;
                    }
                    else
                    {
                        var newContainer = new VirtualSource(path[i]);
                        parent.AddChild(newContainer);
                        parent = newContainer;
                    }
                }

                if (!RealChildren.ContainsKey(topic.Name))
                {
                    var child = CreateContainer(topic.Name, topic.Type);
                    switch (child)
                    {
                    case IRendersToWindow w:
                        w.Title = topic.Name;
                        break;
                    case TrackedObjectsContainer t:
                        t.ObjectLabel = topic.Name;
                        break;
                    }

                    RealChildren[topic.Name] = child;
                    
                    foreach (var renderer in _consumers)
                    {
                        RealChildren[topic.Name].AddConsumer(renderer);
                    }
                }
                else
                {
                    RealChildren[topic.Name].DisplayName = topic.Name.Split('/').Last();
                }

                parent.AddChild(RealChildren[topic.Name]);
            }
        }

        #endregion
    }
}