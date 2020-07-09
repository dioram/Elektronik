using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;


namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamLine : IEquatable<SlamLine>, IComparable<SlamLine>
    {
        public readonly SlamPoint pt1;
        public readonly SlamPoint pt2;

        public SlamLine(int id1, int id2)
        {
            pt1 = new SlamPoint() { id = id1 };
            pt2 = new SlamPoint() { id = id2 };
        }

        public SlamLine(SlamPoint pt1, SlamPoint pt2)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
        }

        public int CompareTo(SlamLine other) => pt1.id.CompareTo(other.pt1.id);

        public bool Equals(SlamLine other) =>
            (pt1.id == other.pt1.id && pt2.id == other.pt2.id) ||
            (pt1.id == other.pt2.id && pt2.id == other.pt1.id);
    }
}
