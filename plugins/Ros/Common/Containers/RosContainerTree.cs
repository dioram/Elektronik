using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;

namespace Elektronik.RosPlugin.Common.Containers
{
    public abstract class RosContainerTree : VirtualContainer
    {
        public (string Name, string Type)[]? ActualTopics;
        public readonly Dictionary<string, IContainerTree> RealChildren = new();

        public RosContainerTree(string displayName) : base(displayName)
        {
        }

        public override void Clear()
        {
            ActualTopics = null;
            base.Clear();
            RealChildren.Clear();
        }

        public override void SetRenderer(object renderer)
        {
            _renderers.Add(renderer);
            base.SetRenderer(renderer);
        }

        #region Protected

        protected abstract IContainerTree CreateContainer(string topicName, string topicType);

        protected virtual void RebuildTree()
        {
            BuildTree();
            Squeeze();
        }

        #endregion

        #region Private

        private readonly List<object> _renderers = new ();

        private void BuildTree()
        {
            if (ActualTopics == null) return;
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
                        parent.AddChild(path[i], newContainer);
                        parent = newContainer;
                    }
                }

                if (!RealChildren.ContainsKey(topic.Name))
                {
                    RealChildren[topic.Name] = CreateContainer(topic.Name, topic.Type);
                    
                    foreach (var renderer in _renderers)
                    {
                        RealChildren[topic.Name].SetRenderer(renderer);
                    }
                }

                parent.AddChild(topic.Name, RealChildren[topic.Name]);
            }
        }

        #endregion
    }
}