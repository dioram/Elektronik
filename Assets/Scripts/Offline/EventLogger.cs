using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Offline
{
    public class EventLogger : MonoBehaviour
    {
        public Text commonIformation;
        public UIListBox listBoxWithSpecializedObjects;
        public SpecialInfoListBoxItem itemPrefab;
        public ObjectLogger loggerForSpecialInformation;

        private void Start()
        {
            listBoxWithSpecializedObjects.OnSelectionChanged += ObjectInfoSelectionChanged;
        }

        public void Clear()
        {
            commonIformation.text = "";
            listBoxWithSpecializedObjects.Clear();
            
        }

        public void UpdateInfo(Package package, ISlamContainer<SlamPoint> pointsMap, SlamObservationsGraph graph)
        {
            listBoxWithSpecializedObjects.Clear();

            commonIformation.text = package.Summary();

            SlamPoint[] specialPts = package.Points.Where(p => p.id != -1).Where(p => p.message != null).ToArray();
            Vector3[] ptsPositionsFromMap = specialPts.Select(p => pointsMap.Get(p.id).position).ToArray();

            SlamObservation[] specialObs = package.Observations
                .Where(o => o.Point.id != -1)
                .Where(o => o.Point.message != null)
                .ToArray();
            Vector3[] obsPositionsFromMap = specialObs.Select(o => graph.Get(o.Point.id).Point.position).ToArray();

            for (int i = 0; i < specialPts.Length; ++i)
            {
                var item = listBoxWithSpecializedObjects.Add(itemPrefab) as SpecialInfoListBoxItem;
                item.SetObject(specialPts[i].id, "Point", ptsPositionsFromMap[i], specialPts[i].message);
            }
            for (int i = 0; i < specialObs.Length; ++i)
            {
                var item = listBoxWithSpecializedObjects.Add(itemPrefab) as SpecialInfoListBoxItem;
                item.SetObject(specialObs[i].Point.id, "Observation", obsPositionsFromMap[i], specialObs[i].Point.message);
            }
        }

        private void ObjectInfoSelectionChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var item = listBoxWithSpecializedObjects[e.index] as SpecialInfoListBoxItem;
            loggerForSpecialInformation.ShowObjectInformation(item.Message, item.Position);
        }
    }
}
