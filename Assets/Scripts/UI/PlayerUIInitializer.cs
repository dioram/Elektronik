using System.Linq;
using Elektronik.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class PlayerUIInitializer : MonoBehaviour
    {
        public Button VrModeButton;
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
            _vrModeKeyPressedEvent = VRMode.GetComponents<KeyPressedEventInvoker>()
                    .First(@event => @event.key == KeyCode.F12);
            _vrModeKeyPressedEvent.myEvent.AddListener(InverseActive);
            VrModeButton.onClick.AddListener(InverseActive);
        }

        void OnDestroy()
        {
            VrModeButton.onClick.RemoveListener(InverseActive);
            _vrModeKeyPressedEvent.myEvent.RemoveListener(InverseActive);
        }
    }
}