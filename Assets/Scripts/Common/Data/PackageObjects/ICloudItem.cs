namespace Elektronik.Common.Data.PackageObjects
{
    public interface ICloudItem
    {
        /// <summary> Unique ID for item </summary>
        int Id { get; set; }
        
        /// <summary> Additional info. </summary>
        string Message { get; set; }

        public SlamPoint AsPoint();
    }
}