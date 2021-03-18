using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Settings;

namespace Elektronik.RosPlugin.Common.Containers
{
    public abstract class RosContainerTree : VirtualContainer
    {
        public (string Name, string Type)[]? ActualTopics;
        public readonly Dictionary<string, IContainerTree> RealChildren = new();
        
        public RosContainerTree(string displayName) : base(displayName)
        {
        }

        public virtual void Init(FileScaleSettingsBag settings)
        {
            DisplayName = settings.FilePath.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag: /";
            BuildTree();
            Squeeze();

            foreach (var child in Children)
            {
                foreach (var renderer in _renderers)
                {
                    child.SetRenderer(renderer);
                }
            }
        }

        public virtual void Reset()
        {
            Clear();
            ChildrenList.Clear();
        }

        public override void SetRenderer(object renderer)
        {
            _renderers.Add(renderer);
            base.SetRenderer(renderer);
        }

        #region Protected
        
        public abstract IContainerTree CreateContainer(string topicName, string topicType);

        #endregion

        #region Private

        private readonly List<object> _renderers = new List<object>();

        private void BuildTree()
        {
            if (ActualTopics == null) return;
            
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

                var realChild = CreateContainer(topic.Name, topic.Type);
                parent.AddChild(topic.Name, realChild);
                RealChildren[topic.Name] = realChild;
            }
        }

        #endregion
        
    }
}