using System;
using JetBrains.Annotations;

namespace Elektronik.PluginsSystem
{
    public interface IDataSourcePluginOffline : IDataSourcePlugin
    {
        /// <summary> Amount of frames (commands) in file. </summary>
        int AmountOfFrames { get; }

        /// <summary> Displaying timestamp of current frame. </summary>
        string CurrentTimestamp { get; }

        /// <summary> Number of current frame. </summary>
        int CurrentPosition { get; set; }
        
        /// <summary> Minimal delay between 2 frames when playing (in ms). </summary>
        int DelayBetweenFrames { get; set; }

        /// <summary> Starts playing. </summary>
        void Play();

        /// <summary> Pauses playing. </summary>
        void Pause();

        /// <summary> Stops playing. </summary>
        void StopPlaying();

        /// <summary> Rewind to previous key frame. </summary>
        /// <remarks>
        /// If in your plugin there aren't any key frames then this function should just call <c>PreviousFrame()</c>.
        /// </remarks>
        void PreviousKeyFrame();

        /// <summary> Rewind to next key frame. </summary>
        /// <remarks>
        /// If in your plugin there aren't any key frames then this function should just call <c>NextFrame()</c>.
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