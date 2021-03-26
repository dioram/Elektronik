using System;
using JetBrains.Annotations;

namespace Elektronik.PluginsSystem
{
    public interface IDataSourcePluginOffline : IDataSourcePlugin
    {
        /// <summary> Amount of frames (commands) in file. </summary>
        int AmountOfFrames { get; }

        /// <summary> Displaying timestamp of current frame. </summary>
        int CurrentTimestamp { get; }

        /// <summary> Number of current frame. </summary>
        int CurrentPosition { get; set; }

        /// <summary> Play button pressed handler. </summary>
        void Play();

        /// <summary> Next button pressed handler. </summary>
        void Pause();

        /// <summary> Stop button pressed handler. </summary>
        void StopPlaying();

        /// <summary> Previous frame button pressed handler. </summary>
        void PreviousKeyFrame();

        /// <summary> Next frame button pressed handler. </summary>
        void NextKeyFrame();

        [CanBeNull] event Action<bool> Rewind;

        /// <summary> Reached end of the file. </summary>
        [CanBeNull] event Action Finished;
    }
}