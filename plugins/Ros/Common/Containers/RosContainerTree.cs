using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.UI.Windows;

namespace Elektronik.RosPlugin.Common.Containers
{
    public abstract class RosContainerTree : VirtualContainer, IDisposable
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

        public override void AddRenderer(ISourceRenderer renderer)
        {
            _renderers.Add(renderer);
            base.AddRenderer(renderer);
        }

        public override void RemoveRenderer(ISourceRenderer renderer)
        {
            _renderers.Remove(renderer);
            base.RemoveRenderer(renderer);
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

        private readonly List<ISourceRenderer> _renderers = new ();

        private void BuildTree()
        {
            ChildrenList.Clear();

            foreach (var topic in ActualTopics)
            {
                var path = topic.Name.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                VirtualContainer parent = this;
                for (int i = 0; i < path.Length - 1; i++)
                {
                    if (parent.Children.FirstOrDefault(c => c.DisplayName == path[i]) is VirtualContainer container)
                    {
                        parent = container;
                    }
                    else
                    {
                        var newContainer = new VirtualContainer(path[i]);
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
                    
                    foreach (var renderer in _renderers)
                    {
                        RealChildren[topic.Name].AddRenderer(renderer);
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