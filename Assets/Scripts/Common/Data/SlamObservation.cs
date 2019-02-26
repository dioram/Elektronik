using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class SlamObservation : ISlamObject
    {
        public Quaternion orientation;
        public byte statistics1;
        public byte statistics2;
        public byte statistics3;
        public byte statistics4;

        public List<int> covisibleObservationsIds;
        public List<int> covisibleObservationsOfCommonPointsCount;

        public SlamObservation()
        {
            covisibleObservationsIds = new List<int>();
            covisibleObservationsOfCommonPointsCount = new List<int>();
        }

        public SlamObservation(SlamObservation src, bool sharedCovisibleObservations = true) : this()
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
            message = src.message;
            covisibleObservationsIds = src.covisibleObservationsIds;
            covisibleObservationsOfCommonPointsCount = src.covisibleObservationsOfCommonPointsCount;
            if (src.covisibleObservationsIds != null && !sharedCovisibleObservations)
                covisibleObservationsIds = new List<int>(src.covisibleObservationsIds);
            if (src.covisibleObservationsOfCommonPointsCount != null && !sharedCovisibleObservations)
                covisibleObservationsOfCommonPointsCount = new List<int>(src.covisibleObservationsOfCommonPointsCount);
        }
    }
}
