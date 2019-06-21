using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Offline
{
    public class EventLogger : MonoBehaviour
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
            commonIformation.text = "";
            listBoxWithSpecializedObjects.Clear();

        }

        public void UpdateInfo(
            SlamPackage package,
            ICloudObjectsContainer<SlamPoint> pointsMap,
            ICloudObjectsContainer<SlamObservation> graph)
        {
            listBoxWithSpecializedObjects.Clear();

            commonIformation.text = package.ToString();

            SlamPoint[] specialPts = package.Points.Where(p => p.id != -1).Where(p => p.message != null).ToArray();
            Vector3[] ptsPositionsFromMap = specialPts.Select(p => pointsMap[p].position).ToArray();

            SlamObservation[] specialObs = package.Observations
                .Where(o => o.Point.id != -1)
                .Where(o => o.Point.message != null)
                .ToArray();
            Vector3[] obsPositionsFromMap = specialObs.Select(o => graph[o].Point.position).ToArray();

            for (int i = 0; i < specialPts.Length; ++i)
            {
                var item = listBoxWithSpecializedObjects.Add() as SpecialInfoListBoxItem;
                item.SetObject(specialPts[i].id, specialPts[i].ToString(), ptsPositionsFromMap[i], specialPts[i].message);
            }
            for (int i = 0; i < specialObs.Length; ++i)
            {
                var item = listBoxWithSpecializedObjects.Add() as SpecialInfoListBoxItem;
                item.SetObject(specialObs[i].Point.id, specialObs[i].ToString(), obsPositionsFromMap[i], specialObs[i].Point.message);
            }
        }

        private void ObjectInfoSelectionChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var item = listBoxWithSpecializedObjects[e.index] as SpecialInfoListBoxItem;
            loggerForSpecialInformation.ShowObjectInformation(item.Message, item.Position);
        }
    }
}
