namespace Elektronik.PluginsSystem
{
    /// <summary> This interface marks that data source plugin can change playback speed. </summary>
    public interface IChangingSpeed
    {
        /// <summary> Current playback speed. </summary>
        float Speed { get; set; }
    }
}