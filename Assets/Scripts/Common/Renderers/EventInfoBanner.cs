using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Elektronik.Common.Renderers
{
    public class EventInfoBanner : MonoBehaviour, IDataRenderer<(string info, string objectType, IEnumerable<SlamPoint> objects)>
    {
        public Text commonIformation;
        public UIListBox listBoxWithSpecializedObjects;
        public ObjectLogger loggerForSpecialInformation;

        private void Start()
        {
            listBoxWithSpecializedObjects.OnSelectionChanged += ObjectInfoSelectionChanged;
        }

        public void Clear()
        {
            commonIformation.text = string.Empty;
            listBoxWithSpecializedObjects.Clear();
        }

        private void ObjectInfoSelectionChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var item = listBoxWithSpecializedObjects[e.Index] as SpecialInfoListBoxItem;
            loggerForSpecialInformation.ShowObjectInformation(item.Message, item.Position);
        }

        public void Render((string info, string objectType, IEnumerable<SlamPoint> objects) data)
        {
            commonIformation.text = data.info;
            
            if (data.objectType == null || data.objects == null) return;
            
            foreach (var obj in data.objects)
            {
                var item = listBoxWithSpecializedObjects.Add() as SpecialInfoListBoxItem;
                item.SetObject(obj.Id, data.objectType, obj.Position, obj.Message);
            }
        }
    }
}
