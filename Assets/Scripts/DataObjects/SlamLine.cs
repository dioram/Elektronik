using System;

namespace Elektronik.DataObjects
{
    /// <summary> Colored line between two <see cref="SlamPoint"/>s. </summary>
    [Serializable]
    public struct SlamLine : IComparable<SlamLine>, ICloudItem
    {
        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public SlamPoint ToPoint() => throw new InvalidCastException("Cannot get line as point");

        /// <summary> Point of line start. </summary>
        public SlamPoint Point1;
        
        /// <summary> Point of line end. </summary>
        public SlamPoint Point2;

        public SlamLine(int id1, int id2, int id = 0)
        {
            Point1 = new SlamPoint(id1);
            Point2 = new SlamPoint(id2);
            Id = id;
            Message = "";
        }

        public SlamLine(SlamPoint point1, SlamPoint point2, int id = 0)
        {
            Point1 = point1;
            Point2 = point2;
            Id = id;
            Message = "";
        }

        /// <summary> Get ids of points. </summary>
        /// <returns></returns>
        public (int Id1, int Id2) GetIds()
        {
            return (Point1.Id, Point2.Id);
        }

        /// <inheritdoc />
        public int CompareTo(SlamLine other) => GetInternalID().CompareTo(other.GetInternalID());


        /// <inheritdoc cref="Equals(object)" />
        public bool Equals(SlamLine other) => GetInternalID() == other.GetInternalID();

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SlamLine other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() => GetInternalID().GetHashCode();

        private ulong GetInternalID()
        {
            var id1 = Math.Min(Point1.Id, Point2.Id);
            var id2 = Math.Max(Point1.Id, Point2.Id);
            return ((ulong)id1 << sizeof(int) * 8) + (ulong)id2;
        }
    }
}
