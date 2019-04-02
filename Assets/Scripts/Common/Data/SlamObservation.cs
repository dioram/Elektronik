using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

namespace Elektronik.Common.Data
{
    public class SlamObservation
    {
        public struct Stats
        {
            public byte statistics1;
            public byte statistics2;
            public byte statistics3;
            public byte statistics4;
        }

        public struct CovisibleObservationInfo
        {
            public int id;
            public int sharedPointsCount;
        }

        public SlamPoint Point { get; set; }
        public Quaternion Orientation { get; set; }
        public Stats Statistics { get; set; }

        public List<CovisibleObservationInfo> CovisibleObservationsInfo { get; private set; }

        public SlamObservation()
        {
            CovisibleObservationsInfo = new List<CovisibleObservationInfo>();
        }

        public SlamObservation(List<CovisibleObservationInfo> covisibleObservationsInfo)
        {
            CovisibleObservationsInfo = covisibleObservationsInfo;
        }
        
        public SlamObservation(SlamObservation src, bool sharedCovisibleObservations = true)
        {
            Point = src.Point;
            Orientation = src.Orientation;
            Statistics = src.Statistics;

            CovisibleObservationsInfo = src.CovisibleObservationsInfo;
            if (src.CovisibleObservationsInfo != null && !sharedCovisibleObservations)
                CovisibleObservationsInfo = new List<CovisibleObservationInfo>(src.CovisibleObservationsInfo);
        }

        public static implicit operator SlamPoint(SlamObservation obs)
        {
            return obs.Point;
        }
    }
}
