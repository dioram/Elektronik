﻿using System;
using Elektronik.Common.Presenters;

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
        
        DataPresenter PresentersChain { get; }
    }
}