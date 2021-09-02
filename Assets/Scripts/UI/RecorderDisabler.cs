using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using UnityEngine;

namespace Elektronik.UI
{
    public class RecorderDisabler : MonoBehaviour
    {
        [SerializeField] private PluginsPlayer Player;
        
        private void Start()
        {
            Player.PluginsStarted += () =>
            {
                if (!PluginsPlayer.Plugins.OfType<IDataRecorderPlugin>().Any()) gameObject.SetActive(false);
            };
        }
    }
}