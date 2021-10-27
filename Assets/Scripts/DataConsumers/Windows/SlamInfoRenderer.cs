using System.Collections.Generic;
using Elektronik.DataObjects;
using Elektronik.UI.ListBox;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.Windows
{
    /// <summary> This class renders additional information about cloud objects. </summary>
    public class SlamInfoRenderer : MonoBehaviour, IDataRenderer<(string message, IEnumerable<ICloudItem> points)>
    {
        #region Editor fields

        /// <summary> Label for packet's messages. </summary>
        [SerializeField] [Tooltip("Label for packet's messages.")]
        private TMP_Text MessageLabel;

        /// <summary> Label for chosen point's messages. </summary>
        [SerializeField] [Tooltip("Label for chosen point's messages.")]
        private TMP_Text PointMessageLabel;

        /// <summary> List of buttons for every point. </summary>
        [SerializeField] [Tooltip("List of buttons for every point.")]
        private ListBox PointButtonsBox;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            IsShowing = true;
        }

        private void OnDisable()
        {
            IsShowing = false;
        }

        #endregion

        #region IDataRenderer

        /// <inheritdoc />
        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                if (_isShowing == value) return;
                _isShowing = value;
                UniRxExtensions.StartOnMainThread(() => gameObject.SetActive(_isShowing)).Subscribe();
            }
        }

        /// <inheritdoc />
        public void Render((string message, IEnumerable<ICloudItem> points) data)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                MessageLabel.text = $"Package message: {data.message}";
                foreach (var point in data.points)
                {
                    var lbi = (SpecialInfoListBoxItem)PointButtonsBox.Add();
                    lbi.Point = point;
                    lbi.MessageLabel = PointMessageLabel;
                }
            }).Subscribe();
        }

        /// <inheritdoc />
        public void Clear()
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                if (MessageLabel != null) MessageLabel.text = "";
                if (PointMessageLabel != null) PointMessageLabel.text = "";
                if (PointButtonsBox != null) PointButtonsBox.Clear();
            }).Subscribe();
        }

        #endregion

        #region Private

        private bool _isShowing;

        #endregion
    }
}