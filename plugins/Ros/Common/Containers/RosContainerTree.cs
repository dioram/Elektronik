using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;

namespace Elektronik.RosPlugin.Common.Containers
{
    public abstract class RosContainerTree : VirtualDataSource, IDisposable
    {
        public List<(string Name, string Type)> ActualTopics = new ();
        public readonly Dictionary<string, IDataSource> RealChildren = new();

        protected RosContainerTree(string displayName) : base(displayName)
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

        protected abstract IDataSource CreateContainer(string topicName, string topicType);

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

            foreach (var (topicName, topicType) in ActualTopics)
            {
                var path = topicName.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                VirtualDataSource parent = this;
                for (var i = 0; i < path.Length - 1; i++)
                {
                    if (parent.Children.FirstOrDefault(c => c.DisplayName == path[i]) is VirtualDataSource container)
                    {
                        parent = container;
                    }
                    else
                    {
                        var newContainer = new VirtualDataSource(path[i]);
                        parent.AddChild(newContainer);
                        parent = newContainer;
                    }
                }

                if (!RealChildren.ContainsKey(topicName))
                {
                    var child = CreateContainer(topicName, topicType);
                    if (child is TrackedCloudObjectsContainer t) t.ObjectLabel = topicName;

                    RealChildren[topicName] = child;
                    
                    foreach (var renderer in _consumers)
                    {
                        RealChildren[topicName].AddConsumer(renderer);
                    }
                }
                else
                {
                    RealChildren[topicName].DisplayName = topicName.Split('/').Last();
                }

                parent.AddChild(RealChildren[topicName]);
            }
        }

        #endregion
    }
}