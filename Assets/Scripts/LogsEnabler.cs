using UnityEngine;
using UnityEngine.UI;

namespace Elektronik
{
    [RequireComponent(typeof(Toggle))]
    public class LogsEnabler : MonoBehaviour
    {
        private void Awake()
        {
            Debug.unityLogger.logEnabled = Debug.isDebugBuild;
            GetComponent<Toggle>().isOn = Debug.isDebugBuild;
        }

        public void ToggleLogger(bool enable)
        {
            Debug.unityLogger.logEnabled = enable;
        }
    }
}