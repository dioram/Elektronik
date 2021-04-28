using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elektronik.Clouds;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.UI;
using Elektronik.UI.Localization;
using Elektronik.UI.Windows;
using UnityEngine;

namespace Elektronik.Data
{
    public class DataSourcesManager : MonoBehaviour
    {
        private static int _snapshotsCount = 0;
        [SerializeField] private RectTransform SourceTreeView;
        [SerializeField] private GameObject TreeElementPrefab;
        [SerializeField] private GameObject Renderers;

        private readonly List<ISourceTree> _dataSources = new List<ISourceTree>();
        private readonly List<SourceTreeElement> _roots = new List<SourceTreeElement>();
        private Component[] _renderers;

        public void MapSourceTree(Action<ISourceTree, string> action)
        {
            foreach (var treeElement in _dataSources)
            {
                MapSourceTree(treeElement, "", action);
            }
        }

        private void Awake()
        {
            _renderers = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.GetInterfaces()
                                   .Where(i => i.IsGenericType)
                                   .Any(i => i.GetGenericTypeDefinition() == typeof(ICloudRenderer<>)))
                    .SelectMany(t => Renderers.GetComponentsInChildren(t))
                    .Concat(new[] {Renderers.transform.Find("Windows").GetComponent<WindowsManager>()})
                    .ToArray();
        }

        public void AddDataSource(ISourceTree source)
        {
            _dataSources.Add(source);
            var treeElement = Instantiate(TreeElementPrefab, SourceTreeView).GetComponent<SourceTreeElement>();
            _roots.Add(treeElement);
            treeElement.Node = source;
            if (_roots.Count == 1)
            {
                treeElement.ChangeState();
            }

            if (source is IRemovable r)
            {
                r.OnRemoved += () => _dataSources.Remove(source);
            }

            // ReSharper disable once LocalVariableHidesMember
            foreach (var renderer in _renderers)
            {
                source.SetRenderer(renderer);
            }
        }

        public void TakeSnapshot()
        {
            // ReSharper disable once LocalVariableHidesMember
            var name = TextLocalizationExtender.GetLocalizedString("Snapshot", _snapshotsCount++);
            var snapshot = new SnapshotContainer(name, _dataSources
                                                         .OfType<ISnapshotable>()
                                                         .Select(s => s.TakeSnapshot())
                                                         .Select(s => s as ISourceTree)
                                                         .ToList());
            AddDataSource(snapshot);
        }
        
        
        private static void MapSourceTree(ISourceTree treeElement, string path, Action<ISourceTree, string> action)
        {
            var fullName = $"{path}/{treeElement.DisplayName}";
            action(treeElement, fullName);
            foreach (var child in treeElement.Children)
            {
                MapSourceTree(child, fullName, action);
            }
        }
    }
}