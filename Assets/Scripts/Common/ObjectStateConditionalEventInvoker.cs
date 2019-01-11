using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Elektronik.Common
{
    public class ObjectStateConditionalEventInvoker : MonoBehaviour
    {
        [Serializable]
        public sealed class ObjectStateConditionalEvent : UnityEvent { }

        public GameObject objectWithDesiredState;

        public ObjectStateConditionalEvent invokeIfTrue;
        public ObjectStateConditionalEvent invokeIfFalse;

        public void Raise()
        {
            if (objectWithDesiredState.activeInHierarchy)
                invokeIfTrue.Invoke();
            else
                invokeIfFalse.Invoke();
        }

    }
}
