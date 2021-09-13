using System;

namespace Elektronik.Data.PackageObjects
{
    [Serializable]
    public struct SlamLine : IComparable<SlamLine>, ICloudItem
    {
        public int Id { get; set; }
        public string Message { get; set; }
        
        public SlamPoint AsPoint()
        {
            throw new InvalidCastException("Cannot get line as point");
        }

        public SlamPoint Point1;
        public SlamPoint Point2;

        public SlamLine(int id1, int id2, int id = 0)
        {
            Point1 = new SlamPoint() { Id = id1 };
            Point2 = new SlamPoint() { Id = id2 };
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

        public (int Id1, int Id2) GetIds()
        {
            return (Point1.Id, Point2.Id);
        }

        public int CompareTo(SlamLine other) => GetInternalID().CompareTo(other.GetInternalID());

        public bool Equals(SlamLine other) => GetInternalID() == other.GetInternalID();

        private ulong GetInternalID()
        {
            if (Point1.Id < Point2.Id)
                return ((ulong)Point1.Id << sizeof(int) * 8) + (ulong)Point2.Id;
            return ((ulong)Point2.Id << sizeof(int) * 8) + (ulong)Point1.Id;
        }
    }
}
