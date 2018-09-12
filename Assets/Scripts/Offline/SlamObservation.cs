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
    }
}
