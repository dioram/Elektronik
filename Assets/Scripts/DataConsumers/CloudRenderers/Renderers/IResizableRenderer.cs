namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Interface for renderers that can resize rendered objects. </summary>
    internal interface IResizableRenderer
    {
        /// <summary> Size of rendered objects. </summary>
        float ItemSize { get; set; }
    }
}