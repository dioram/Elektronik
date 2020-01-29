using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.UI;
using Elektronik.Common.Data.Packages;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Elektronik.Common.Data;

namespace Elektronik.Offline.Loggers
{
    public class EventInfoBanner : MonoBehaviour
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

        public void UpdateCommonInfo(string info)
        {
            commonIformation.text = info;
        }

        public void UpdateExtraInfo(string objectType, IEnumerable<SlamPoint> objects)
        {
            foreach (var obj in objects)
            {
                var item = listBoxWithSpecializedObjects.Add() as SpecialInfoListBoxItem;
                item.SetObject(obj.id, objectType, obj.position, obj.message);
            }
        }

        private void ObjectInfoSelectionChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var item = listBoxWithSpecializedObjects[e.index] as SpecialInfoListBoxItem;
            loggerForSpecialInformation.ShowObjectInformation(item.Message, item.Position);
        }
    }
}
