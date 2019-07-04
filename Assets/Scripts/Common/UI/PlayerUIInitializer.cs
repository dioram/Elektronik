using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Elektronik.Common.Events;

namespace Elektronik.Common.UI
{
    public class PlayerUIInitializer : MonoBehaviour
    {
        Button m_VRModeButton;
        KeyPressedEventInvoker m_VRModeKeyPressedEvent;

        public GameObject VRMode;
        public GameObject NonVRMode;

        void InverseActive()
        {
            gameObject.SetActive(!isActiveAndEnabled);
        }

        // Start is called before the first frame update
        void Start()
        {
            m_VRModeButton = NonVRMode.transform
                .Find(@"UIControls/Control panel/Control elements/VR Mode Button")
                .GetComponent<Button>();
            m_VRModeKeyPressedEvent = VRMode.GetComponents<KeyPressedEventInvoker>().First(@event => @event.key == KeyCode.F12);
            m_VRModeKeyPressedEvent.myEvent.AddListener(InverseActive);
            m_VRModeButton.onClick.AddListener(InverseActive);
        }

        void OnDestroy()
        {
            m_VRModeButton.onClick.RemoveListener(InverseActive);
            m_VRModeKeyPressedEvent.myEvent.RemoveListener(InverseActive);
        }
    }
}