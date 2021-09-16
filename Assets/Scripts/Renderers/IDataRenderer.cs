namespace Elektronik.Renderers
{
    /// <summary>
    /// Interface for renderers that work with data that can't be rendered on scene such as text, tables, images etc.
    /// </summary>
    /// <typeparam name="T"> Type of data to render. </typeparam>
    public interface IDataRenderer<in T> : ISourceRenderer
    {
        /// <summary> Is renderer visible. </summary>
        bool IsShowing { get; set; }
        
        /// <summary> Renders incoming data. </summary>
        /// <param name="data"> Data to render. </param>
        void Render(T data);
        
        /// <summary> Clear rendered data. </summary>
        void Clear();
    }
}