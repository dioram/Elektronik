using System;
using UnityEngine;
using UnityEngine.Events;


namespace Elektronik.Common

{
    public class KeyPressedEventInvoker : MonoBehaviour
    {
        [Serializable]
        public sealed class KeyPressedEvent : UnityEvent
        {
        }

        public KeyCode key;
        public KeyPressedEvent myEvent;

        public void Update()
        {
            if (Input.GetKeyDown(key))
            {
                myEvent.Invoke();
            }
        }
    }
}