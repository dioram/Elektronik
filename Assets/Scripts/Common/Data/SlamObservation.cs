using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

namespace Elektronik.Common.Data
{
    public class SlamObservation
    {
        public SlamPoint Point { get; set; }
        public Quaternion Orientation { get; set; }

        public struct Stats
        {
            public byte statistics1;
            public byte statistics2;
            public byte statistics3;
            public byte statistics4;
        }
        public Stats Statistics { get; set; }

        public struct CovisibleInfo
        {
            public int id;
            public int sharedPointsCount;
        }
        public ReadOnlyCollection<CovisibleInfo> CovisibleInfos { get; private set; }

        public SlamObservation(List<CovisibleInfo> covisibleObservationsInfo)
        {
            CovisibleInfos = new ReadOnlyCollection<CovisibleInfo>(covisibleObservationsInfo);
        }
        
        public SlamObservation(SlamObservation src)
        {
            Point = src.Point;
            Orientation = src.Orientation;
            Statistics = src.Statistics;
            CovisibleInfos = src.CovisibleInfos;
        }

        public static implicit operator SlamPoint(SlamObservation obs)
        {
            return obs.Point;
        }
    }
}
