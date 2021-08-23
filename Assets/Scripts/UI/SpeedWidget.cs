using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.Settings;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class SpeedWidget : MonoBehaviour
    {
        [SerializeField] private Slider SpeedSlider;

        private void Start()
        {
            if (ModeSelector.Mode != Mode.Offline) gameObject.SetActive(false);
            SpeedSlider.OnValueChangedAsObservable()
                       .Select(v => 1 + (int)SpeedSlider.maxValue - (int)v)
                       .Do(SetDelay)
                       .Subscribe();
        }

        private void SetDelay(int value)
        {
            foreach (var plugin in PluginsPlayer.Plugins.OfType<IDataSourcePluginOffline>())
            {
                plugin.DelayBetweenFrames = value;
            }
        }
    }
}