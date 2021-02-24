using System;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Offline
{
    public class PlayerEventsManager : MonoBehaviour, IPlayerEvents
    {
        public event Action Play;
        public event Action Pause;
        public event Action Stop;
        public event Action PreviousFrame;
        public event Action NextFrame;
        public event Action PreviousKeyFrame;
        public event Action NextKeyFrame;
        public event Action<float> RewindAt;
        public int CurrentTimestamp { get; set; }

        public Button PlayButton;
        public Button PauseButton;
        public Button StopButton;
        public Button NextKeyFrameButton;
        public Button PreviousKeyFrameButton;
        public Slider TimelineSlider;

        private void Awake()
        {
            PlayButton.onClick.AddListener(() => Play?.Invoke());
            PauseButton.onClick.AddListener(() => Pause?.Invoke());
            StopButton.onClick.AddListener(() => Stop?.Invoke());
            NextKeyFrameButton.onClick.AddListener(() => NextKeyFrame?.Invoke());
            PreviousKeyFrameButton.onClick.AddListener(() => PreviousKeyFrame?.Invoke());
            TimelineSlider.onValueChanged.AddListener((delegate(float arg0) { RewindAt?.Invoke(arg0); }));
        }
    }
}