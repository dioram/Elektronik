using UnityEngine;
using UnityEngine.UI;

namespace Elektronik
{
    /// <summary> Class for enabling or disabling logging to file using <see cref="UnityEngine.Debug"/> </summary>
    [RequireComponent(typeof(Toggle))]
    internal class LogsEnabler : MonoBehaviour
    {
        public void ToggleLogger(bool enable)
        {
            Debug.unityLogger.logEnabled = enable;
        }
        
        private void Awake()
        {
            Debug.unityLogger.logEnabled = Debug.isDebugBuild;
            GetComponent<Toggle>().isOn = Debug.isDebugBuild;
        }
    }
}