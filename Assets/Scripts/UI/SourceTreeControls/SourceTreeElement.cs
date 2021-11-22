using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataSources;
using Elektronik.UI.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> Component for rendering data sources in data source window. </summary>
    [RequireComponent(typeof(LayoutElement))]
    internal class SourceTreeElement : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private RectTransform Content;
        [SerializeField] private GameObject Spacer;
        [SerializeField] private GameObject SelfPrefab;
        [SerializeField] private int MinHeight = 40;
        [SerializeField] private ButtonChangingIcons TreeButton;

        #endregion

        /// <summary> Model for this view. </summary>
        public IDataSource Node;
        
        /// <summary> This event will be raised if size of UI element was changed. </summary>
        public event Action OnSizeChanged;

        /// <summary> This event will be raised if data source changed it's DisplayName. </summary>
        public event Action<string> OnNameChanged;

        /// <summary> Expands this element and shows it's children. </summary>
        public void Expand()
        {
            if (!_isInitialized) return;
            
            TreeButton.SilentSetState(1);
            var expanded = GetExpandedSize(true);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded);
            _layoutElement.minHeight = expanded;
            Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expanded - MinHeight);
            Content.gameObject.SetActive(true);
            _isExpanded = true;
            
            OnSizeChanged?.Invoke();
        }

        /// <summary> Collapses this element to one line. </summary>
        public void Collapse()
        {
            if (!_isInitialized) return;
            
            TreeButton.SilentSetState(0);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, MinHeight);
            _layoutElement.minHeight = MinHeight;
            _layoutElement.preferredHeight = MinHeight;
            Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            Content.gameObject.SetActive(false);
            _isExpanded = false;
            
            OnSizeChanged?.Invoke();
        }

        #region Unity events

        public void Awake()
        {
            _rectTransform = (RectTransform) transform;
            _layoutElement = GetComponent<LayoutElement>();
            _isInitialized = true;
        }

        public void Start()
        {
            TreeButton.OnStateChanged += s =>
            {
                switch (s)
                {
                case 1:
                    Expand();
                    break;
                case 0:
                    Collapse();
                    break;
                default:
                    Debug.LogWarning($"Button has unsupported state {s}");
                    break;
                }
            };
            StartCoroutine(CheckChildrenChanges());
        }

        #endregion

        #region Private

        private bool _isInitialized = false;
        private bool _isExpanded;
        private RectTransform _rectTransform;
        private LayoutElement _layoutElement;
        private readonly List<SourceTreeElement> _children = new List<SourceTreeElement>();

        private float GetExpandedSize(bool ignoreSelfExpand = false)
        {
            if (_children.Count == 0 || (!_isExpanded && !ignoreSelfExpand)) return MinHeight;

            return MinHeight + _children.Sum(child => child.GetExpandedSize());
        }

        // TODO: somehow rewrite it to events.
        private IEnumerator CheckChildrenChanges()
        {
            while (true)
            {
                lock (Node.Children)
                {
                    OnNameChanged?.Invoke(Node.DisplayName);
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