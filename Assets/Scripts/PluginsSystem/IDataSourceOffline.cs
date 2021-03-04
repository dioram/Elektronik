using System;

namespace Elektronik.PluginsSystem
{
    public interface IDataSourceOffline : IDataSource
    {
        int AmountOfFrames { get; }
        int CurrentTimestamp { get; }
        int CurrentPosition { get; set; }
        
        void Play();
        void Pause();
        void StopPlaying();
        void PreviousKeyFrame();
        void NextKeyFrame();

        event Action Finished;
    }
}