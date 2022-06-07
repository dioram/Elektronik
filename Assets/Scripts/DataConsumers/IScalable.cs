namespace Elektronik.DataConsumers
{
    // TODO: Make it internal
    // For that I need to remove functions AddConsumer and RemoveConsumer from ISourceNode
    
    /// <summary> Interface for Data consumers that can be scaled. </summary>
    public interface IScalable
    {
        /// <summary> Scale of the scene. </summary>
        float Scale { get; set; }
    }
}