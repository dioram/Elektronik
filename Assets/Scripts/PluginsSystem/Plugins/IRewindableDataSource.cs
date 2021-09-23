using System;
using JetBrains.Annotations;

namespace Elektronik.PluginsSystem
{
    public interface IRewindableDataSource : IDataSourcePlugin
    {
        /// <summary> Amount of frames (commands) in file. </summary>
        int AmountOfFrames { get; }

        /// <summary> Displaying timestamp of current frame. </summary>
        string Timestamp { get; }

        /// <summary> Number of current frame. </summary>
        int Position { get; set; }

        bool IsPlaying { get; }

        /// <summary> Starts playing. </summary>
        /// <remarks> Should emit <c>OnPlayingStarted</c>. </remarks>
        void Play();

        /// <summary> Pauses playing. </summary>
        /// <remarks> Should emit <c>OnPaused</c>. </remarks>
        void Pause();

        /// <summary> Stops playing. </summary>
        /// <remarks> Should emit <c>OnPaused</c>. </remarks>
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

        [CanBeNull] event Action OnPlayingStarted;
        [CanBeNull] event Action OnPaused;
        [CanBeNull] event Action<int> OnPositionChanged;
        [CanBeNull] event Action<int> OnAmountOfFramesChanged;
        [CanBeNull] event Action<string> OnTimestampChanged;
        [CanBeNull] event Action OnRewindStarted;
        [CanBeNull] event Action OnRewindFinished;

        /// <summary> Reached end of the file. </summary>
        [CanBeNull] event Action OnFinished;
    }
}