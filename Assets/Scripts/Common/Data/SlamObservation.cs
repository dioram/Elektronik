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

        public struct CovisibleInfo
        {
            public int id;
            public int sharedPointsCount;
        }

        public SlamPoint Point { get; set; }
        public Quaternion Orientation { get; set; }
        public Stats Statistics { get; set; }

        public List<CovisibleInfo> CovisibleInfos { get; private set; }

        public SlamObservation()
        {
            CovisibleInfos = new List<CovisibleInfo>();
        }

        public SlamObservation(List<CovisibleInfo> covisibleObservationsInfo)
        {
            CovisibleInfos = covisibleObservationsInfo;
        }
        
        public SlamObservation(SlamObservation src, bool sharedCovisibleObservations = true)
        {
            Point = src.Point;
            Orientation = src.Orientation;
            Statistics = src.Statistics;

            CovisibleInfos = src.CovisibleInfos;
            if (src.CovisibleInfos != null && !sharedCovisibleObservations)
                CovisibleInfos = new List<CovisibleInfo>(src.CovisibleInfos);
        }

        public static implicit operator SlamPoint(SlamObservation obs)
        {
            return obs.Point;
        }
    }
}
