﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(LayoutElement))]
    public class ContainerTreeElement : MonoBehaviour
    {
        public IContainerTree Node;
        public Button TreeButton;
        public ButtonChangingIcons VisibleButton;
        public Text NameLabel;
        public RectTransform Content;
        public GameObject Spacer;
        public GameObject SelfPrefab;
        public int MinHeight = 40;

        public event Action OnSizeChanged;

        private bool _isExpanded;
        private RectTransform _rectTransform;
        private LayoutElement _layoutElement;
        private readonly List<ContainerTreeElement> _children = new List<ContainerTreeElement>();

        public void Start()
        {
            _rectTransform = (RectTransform) transform;
            _layoutElement = GetComponent<LayoutElement>();
            VisibleButton.OnClickAsObservable()
                    .Subscribe(_ => { Node.IsActive = !Node.IsActive; });
            TreeButton.OnClickAsObservable().Subscribe(_ => ChangeState());
            NameLabel.text = Node.DisplayName;

            StartCoroutine(CheckChildrenChanges());
        }

        public void Update()
        {
            VisibleButton.State = Node.IsActive ? 0 : 1;
        }

        public float GetExpandedSize(bool ignoreSelfExpand = false)
        {
            if (_children.Count == 0 || (!_isExpanded && !ignoreSelfExpand)) return MinHeight;

            float res = MinHeight;
            foreach (var child in _children)
            {
                res += child.GetExpandedSize();
            }

            return res;
        }

        private IEnumerator CheckChildrenChanges()
        {
            while (true)
            {
                lock (Node.Children)
                {
                    var newChildren = Node.Children.Where(c => !_children.Exists(ui => ui.Node == c)).ToList();
                    foreach (var child in newChildren)
                    {
                        var go = Instantiate(SelfPrefab, Content);
                        var treeElement = go.GetComponent<ContainerTreeElement>();
                        treeElement.Node = child;
                        treeElement.OnSizeChanged += OnChildSizeChanged;
                        _children.Add(treeElement);
                    }

                    var removedChildren = _children.Where(c => !Node.Children.Contains(c.Node)).ToList();
                    foreach (var child in removedChildren)
                    {
                        _children.Remove(child);
                        Destroy(child.gameObject);
                    }

                    if (newChildren.Count + removedChildren.Count != 0)
                    {
                        OnChildSizeChanged();
                        OnSizeChanged?.Invoke();
                    }

                    var hasChildren = _children.Count > 0;
                    Spacer.SetActive(!hasChildren);
                    TreeButton.gameObject.SetActive(hasChildren);
                }

                yield return new WaitForSeconds(1);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void OnChildSizeChanged()
        {
            var expanded = GetExpandedSize();
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded);
            _layoutElement.minHeight = expanded;
            Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded - MinHeight);
        }

        private void ChangeState()
        {
            if (_isExpanded)
            {
                _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, MinHeight);
                _layoutElement.minHeight = MinHeight;
                _layoutElement.preferredHeight = MinHeight;
                Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                Content.gameObject.SetActive(false);
                _isExpanded = false;
            }
            else
            {
                var expanded = GetExpandedSize(true);
                _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded);
                _layoutElement.minHeight = expanded;
                Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded - MinHeight);
                Content.gameObject.SetActive(true);
                _isExpanded = true;
            }

            OnSizeChanged?.Invoke();
        }
    }
}