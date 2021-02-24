using System;

namespace Elektronik.Offline
{
    public interface IPlayerEvents
    {
        event Action Play;
        event Action Pause;
        event Action Stop;
        event Action PreviousFrame;
        event Action NextFrame;
        event Action PreviousKeyFrame;
        event Action NextKeyFrame;
        event Action<float> RewindAt;

        int CurrentTimestamp { get; set; }
    }
}