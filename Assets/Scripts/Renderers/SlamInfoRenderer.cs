using System.Collections.Generic;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.ListBox;
using TMPro;
using UnityEngine;

namespace Elektronik.Renderers
{
    public class SlamInfoRenderer : MonoBehaviour, IDataRenderer<(string message, IEnumerable<ICloudItem> points)>
    {
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

        public bool IsShowing { get; private set; }

        public void Render((string message, IEnumerable<ICloudItem> points) data)
        {
            MessageLabel.text = $"Package message: {data.message}";
            foreach (var point in data.points)
            {
                var lbi = (SpecialInfoListBoxItem) PointButtonsBox.Add();
                lbi.Point = point;
                lbi.MessageLabel = PointMessageLabel;
            }
        }

        public void Clear()
        {
            if (MessageLabel != null) MessageLabel.text = "";
            if (PointMessageLabel != null) PointMessageLabel.text = "";
            if (PointButtonsBox != null) PointButtonsBox.Clear();
        }

        #endregion
        
        #region Private
        
        [SerializeField] private TMP_Text MessageLabel;
        [SerializeField] private TMP_Text PointMessageLabel;
        [SerializeField] private ListBox PointButtonsBox;

        #endregion
    }
}