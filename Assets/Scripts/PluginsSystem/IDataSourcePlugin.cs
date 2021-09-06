using System;
using Elektronik.Data;
using JetBrains.Annotations;

namespace Elektronik.PluginsSystem
{
    public interface IDataSourcePlugin : IElektronikPlugin
    {
        ISourceTree Data { get; }
        
        /// <summary> Amount of frames (commands) in file. </summary>
        int AmountOfFrames { get; }

        /// <summary> Displaying timestamp of current frame. </summary>
        string CurrentTimestamp { get; }

        /// <summary> Number of current frame. </summary>
        int CurrentPosition { get; set; }

        /// <summary> Starts playing. </summary>
        void Play();

        /// <summary> Pauses playing. </summary>
        void Pause();

        /// <summary> Stops playing. </summary>
        void StopPlaying();

        /// <summary> Rewind to previous key frame. </summary>
        /// <remarks>
        /// If your plugin does not support key frames then this function should just call <c>PreviousFrame()</c>.
        /// </remarks>
        void PreviousKeyFrame();

        /// <summary> Rewind to next key frame. </summary>
        /// <remarks>
        /// If your plugin does not support then this function should just call <c>NextFrame()</c>.
        /// </remarks>
        void NextKeyFrame();

        /// <summary> Go to next frame. </summary>
        void PreviousFrame();

        /// <summary> Go to previous frame. </summary>
        void NextFrame();

        [CanBeNull] event Action<bool> Rewind;

        /// <summary> Reached end of the file. </summary>
        [CanBeNull] event Action Finished;
    }
}