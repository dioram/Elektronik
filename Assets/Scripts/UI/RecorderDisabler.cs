using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using UnityEngine;

namespace Elektronik.UI
{
    public class RecorderDisabler : MonoBehaviour
    {
        private void Start()
        {
            if (!PluginsPlayer.Plugins.OfType<IDataRecorderPlugin>().Any()) gameObject.SetActive(false);
        }
    }
}