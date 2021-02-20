using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Elektronik.Common.Events;

namespace Elektronik.Common.UI
{
    public class PlayerUIInitializer : MonoBehaviour
    {
        private Button _vrModeButton;
        private KeyPressedEventInvoker _vrModeKeyPressedEvent;

        public GameObject VRMode;
        public GameObject NonVRMode;

        void InverseActive()
        {
            gameObject.SetActive(!isActiveAndEnabled);
        }

        // Start is called before the first frame update
        void Start()
        {
            _vrModeButton = NonVRMode.transform
                .Find(@"UIControls/Control panel/Control elements/VR Mode Button")
                .GetComponent<Button>();
            _vrModeKeyPressedEvent = VRMode.GetComponents<KeyPressedEventInvoker>().First(@event => @event.key == KeyCode.F12);
            _vrModeKeyPressedEvent.myEvent.AddListener(InverseActive);
            _vrModeButton.onClick.AddListener(InverseActive);
        }

        void OnDestroy()
        {
            _vrModeButton.onClick.RemoveListener(InverseActive);
            _vrModeKeyPressedEvent.myEvent.RemoveListener(InverseActive);
        }
    }
}