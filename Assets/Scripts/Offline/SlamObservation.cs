using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    public struct SlamObservation
    {
        public int id;
        public Vector3 position;
        public Quaternion orientation;
        public bool isRemoved;
        public Color color;
        public byte statistics1;
        public byte statistics2;
        public byte statistics3;
        public byte statistics4;

        public int[] covisibleObservationsIds;
        public int[] covisibleObservationsOfCommonPointsCount;

        public SlamObservation(SlamObservation src)
        {
            id = src.id;
            position = src.position;
            orientation = src.orientation;
            isRemoved = src.isRemoved;
            color = src.color;
            statistics1 = src.statistics1;
            statistics2 = src.statistics2;
            statistics3 = src.statistics3;
            statistics4 = src.statistics4;
            covisibleObservationsIds = null;
            covisibleObservationsOfCommonPointsCount = null;
            if (src.covisibleObservationsIds != null)
                covisibleObservationsIds = src.covisibleObservationsIds.Clone() as int[];
            if (src.covisibleObservationsOfCommonPointsCount != null)
                covisibleObservationsOfCommonPointsCount = src.covisibleObservationsOfCommonPointsCount.Clone() as int[];
        }
    }
}
