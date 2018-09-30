using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    public class SlamObservation
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

        public SlamObservation() { }

        public SlamObservation(SlamObservation src, bool sharedCovisibleObservations = true)
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
            covisibleObservationsIds = src.covisibleObservationsIds;
            covisibleObservationsOfCommonPointsCount = src.covisibleObservationsOfCommonPointsCount;
            if (src.covisibleObservationsIds != null && !sharedCovisibleObservations)
                covisibleObservationsIds = src.covisibleObservationsIds.Clone() as int[];
            if (src.covisibleObservationsOfCommonPointsCount != null && !sharedCovisibleObservations)
                covisibleObservationsOfCommonPointsCount = src.covisibleObservationsOfCommonPointsCount.Clone() as int[];
        }
    }
}
