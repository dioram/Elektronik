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
        private readonly ulong m_id;
        public SlamPoint pt1;
        public SlamPoint pt2;

        public SlamLine(int id1, int id2)
        {
            pt1 = new SlamPoint() { id = id1 };
            pt2 = new SlamPoint() { id = id2 };
            if (id1 < id2)
                m_id = ((ulong)id1 << sizeof(int) * 8) + (ulong)id2;
            else
                m_id = ((ulong)id2 << sizeof(int) * 8) + (ulong)id1;
        }

        public SlamLine(SlamPoint pt1, SlamPoint pt2)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
            if (pt1.id < pt2.id)
                m_id = ((ulong)pt1.id << sizeof(int) * 8) + (ulong)pt2.id;
            else
                m_id = ((ulong)pt2.id << sizeof(int) * 8) + (ulong)pt1.id;
        }

        public int CompareTo(SlamLine other) => m_id.CompareTo(other.m_id);

        public bool Equals(SlamLine other) => m_id == other.m_id;
    }
}
