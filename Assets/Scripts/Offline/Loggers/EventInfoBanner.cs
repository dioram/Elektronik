﻿using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
                item.SetObject(obj.Id, objectType, obj.Position, obj.Message);
            }
        }

        private void ObjectInfoSelectionChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var item = listBoxWithSpecializedObjects[e.Index] as SpecialInfoListBoxItem;
            loggerForSpecialInformation.ShowObjectInformation(item.Message, item.Position);
        }
    }
}
