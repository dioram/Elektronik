using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataSources;
using Elektronik.UI.Buttons;
using Elektronik.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(LayoutElement))]
    public class SourceTreeElement : MonoBehaviour
    {
        public ISourceTreeNode Node;
        public Text NameLabel;
        public RectTransform Content;
        public GameObject Spacer;
        public GameObject SelfPrefab;
        public int MinHeight = 40;

        public event Action OnSizeChanged;

        private bool _isExpanded;
        private RectTransform _rectTransform;
        private LayoutElement _layoutElement;
        private readonly List<SourceTreeElement> _children = new List<SourceTreeElement>();
        public ButtonChangingIcons TreeButton;

        public void ChangeState()
        {
            if (_isExpanded)
            {
                TreeButton.State = 0;
                _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, MinHeight);
                _layoutElement.minHeight = MinHeight;
                _layoutElement.preferredHeight = MinHeight;
                Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                Content.gameObject.SetActive(false);
                _isExpanded = false;
            }
            else
            {
                TreeButton.State = 1;
                var expanded = GetExpandedSize(true);
                _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded);
                _layoutElement.minHeight = expanded;
                Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded - MinHeight);
                Content.gameObject.SetActive(true);
                _isExpanded = true;
            }

            OnSizeChanged?.Invoke();
        }

        #region Unity events

        public void Awake()
        {
            _rectTransform = (RectTransform) transform;
            _layoutElement = GetComponent<LayoutElement>();
        }

        public void Start()
        {
            TreeButton.OnStateChanged += _ => ChangeState();
            NameLabel.SetLocalizedText(Node.DisplayName);

            StartCoroutine(CheckChildrenChanges());
        }

        #endregion

        #region Private

        private float GetExpandedSize(bool ignoreSelfExpand = false)
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
                    NameLabel.SetLocalizedText(Node.DisplayName);
                    var newChildren = Node.Children.Where(c => !_children.Exists(ui => ui.Node == c)).ToList();
                    foreach (var child in newChildren)
                    {
                        var go = Instantiate(SelfPrefab, Content);
                        var treeElement = go.GetComponent<SourceTreeElement>();
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
            OnSizeChanged?.Invoke();
        }

        #endregion
    }
}