namespace Elektronik.Data.PackageObjects
{
    public struct SlamLineDiff : ICloudItemDiff<SlamLine>
    {
        public int Id { get; }
        public SlamPointDiff Point1;
        public SlamPointDiff Point2;
        
        public SlamLine Apply()
        {
            return new SlamLine(Point1.Apply(), Point2.Apply());
        }

        public SlamLine Apply(SlamLine item)
        {
            item.Point1 = Point1.Apply(item.Point1);
            item.Point2 = Point2.Apply(item.Point2);
            return item;
        }
    }
}