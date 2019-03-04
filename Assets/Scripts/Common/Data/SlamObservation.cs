using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class SlamObservation
    {
        public Quaternion orientation;
        public byte statistics1;
        public byte statistics2;
        public byte statistics3;
        public byte statistics4;

        public List<int> covisibleObservationsIds;
        public List<int> covisibleObservationsOfCommonPointsCount;

        public SlamPoint Point { get; set; }

        public SlamObservation()
        {
            covisibleObservationsIds = new List<int>();
            covisibleObservationsOfCommonPointsCount = new List<int>();
        }

        public SlamObservation(SlamObservation src, bool sharedCovisibleObservations = true) : this()
        {
            Point = src.Point;
            orientation = src.orientation;
            statistics1 = src.statistics1;
            statistics2 = src.statistics2;
            statistics3 = src.statistics3;
            statistics4 = src.statistics4;
            covisibleObservationsIds = src.covisibleObservationsIds;
            covisibleObservationsOfCommonPointsCount = src.covisibleObservationsOfCommonPointsCount;
            if (src.covisibleObservationsIds != null && !sharedCovisibleObservations)
                covisibleObservationsIds = new List<int>(src.covisibleObservationsIds);
            if (src.covisibleObservationsOfCommonPointsCount != null && !sharedCovisibleObservations)
                covisibleObservationsOfCommonPointsCount = new List<int>(src.covisibleObservationsOfCommonPointsCount);
        }

        public static implicit operator SlamPoint(SlamObservation obs)
        {
            return obs.Point;
        }
    }
}
