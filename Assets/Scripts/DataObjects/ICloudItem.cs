namespace Elektronik.DataObjects
{
    /// <summary> Interface for any items that could be represent in cloud e.g. points, lines, etc. </summary>
    public interface ICloudItem
    {
        /// <summary> Unique ID for item </summary>
        int Id { get; set; }
        
        /// <summary> Additional info. </summary>
        string Message { get; set; }

        /// <summary> Converts this item to point in 3d space. </summary>
        public SlamPoint ToPoint();
    }
}