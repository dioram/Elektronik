﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.Settings.Bags;
using Elektronik.UI.SettingsFields;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Windows.ConnectionsWindow
{
    public class ConnectionsWindow : MonoBehaviour
    {
        [Header("Targets")] [SerializeField] private RectTransform PluginsTarget;
        [SerializeField] private RectTransform RecentSettingsTarget;
        [Header("Prefabs")] [SerializeField] private GameObject PluginWidgetPrefab;
        [SerializeField] private GameObject RecentSettingsPrefab;
        [Space] [SerializeField] private SettingsGenerator Generator;
        [SerializeField] private Button ConnectButton;

        private readonly List<PluginWidget> _widgets = new List<PluginWidget>();
        private IDataSourcePluginsFactory _selectedFactory;

        public void Start()
        {
            ConnectButton.OnClickAsObservable().Subscribe(_ => _selectedFactory.SaveSettings());
        }

        public void OnEnable()
        {
            Generator.Clear();
            _widgets.Clear();
            DestroyChildren(PluginsTarget);
            DestroyChildren(RecentSettingsTarget);
            foreach (var factory in PluginsLoader.Instance.PluginFactories.OfType<IDataSourcePluginsFactory>())
            {
                var go = Instantiate(PluginWidgetPrefab, PluginsTarget);
                var widget = go.GetComponent<PluginWidget>();
                widget.Setup(factory);
                widget.OnSelected()
                        .Do(p => Generator.Generate(p.Settings))
                        .Do(HideOther)
                        .Do(AddRecent)
                        .Do(_ => SelectFirst())
                        .Do(p => _selectedFactory = p)
                        .Subscribe()
                        .AddTo(widget);
                _widgets.Add(widget);
            }
            StartCoroutine(SelectFirst());
        }

        private IEnumerator SelectFirst()
        {
            yield return new WaitForSeconds(Time.deltaTime * 2);
            if (_widgets.Count > 0) _widgets[0].Select();
        }

        private void HideOther(IDataSourcePluginsFactory plugin)
        {
            foreach (var widget in _widgets.Where(w => w.Plugin != plugin))
            {
                widget.Minimize();
            }
        }

        private void AddRecent(IDataSourcePluginsFactory factory)
        {
            DestroyChildren(RecentSettingsTarget);
            foreach (var bag in factory.SettingsHistory.Recent)
            {
                var go = Instantiate(RecentSettingsPrefab, RecentSettingsTarget);
                var widget = go.GetComponent<RecentSettingsWidget>();
                widget.Setup(bag.Clone());
                widget.OnSelected()
                        .Do(b => factory.Settings = b.Clone())
                        .Do(_ => Generator.Generate(factory.Settings))
                        .Subscribe()
                        .AddTo(widget);
            }
        }

        private static void DestroyChildren(Transform tr)
        {
            var count = tr.childCount;
            for (var i = count - 1; i >= 0; i--)
            {
                Destroy(tr.GetChild(i).gameObject);
            }
        }
    }
}