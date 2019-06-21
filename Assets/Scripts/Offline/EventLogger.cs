using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.UI;
using System.Collections.Generic;
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

        private Stack<SlamPackage> m_updateHistory;

        private void Awake()
        {
            m_updateHistory = new Stack<SlamPackage>();
        }

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
            m_updateHistory.Push(package);
            if (package == null)
            {
                commonIformation.text = "";
                return;
            }

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

        public void RestoreInfo(
            ICloudObjectsContainer<SlamPoint> pointsMap,
            ICloudObjectsContainer<SlamObservation> graph)
        {
            if (m_updateHistory.Count > 0)
                m_updateHistory.Pop();
            if (m_updateHistory.Count > 0)
                UpdateInfo(m_updateHistory.Peek(), pointsMap, graph);
            else
                UpdateInfo(null, pointsMap, graph);
        }

        private void ObjectInfoSelectionChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var item = listBoxWithSpecializedObjects[e.index] as SpecialInfoListBoxItem;
            loggerForSpecialInformation.ShowObjectInformation(item.Message, item.Position);
        }
    }
}
