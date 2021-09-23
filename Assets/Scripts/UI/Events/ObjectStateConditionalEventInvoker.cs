using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Elektronik.UI.Events
{
    public class ObjectStateConditionalEventInvoker : MonoBehaviour
    {
        [Serializable]
        public sealed class ObjectStateConditionalEvent : UnityEvent { }

        [FormerlySerializedAs("objectWithDesiredState")] public GameObject ObjectWithDesiredState;

        [FormerlySerializedAs("invokeIfTrue")] public ObjectStateConditionalEvent InvokeIfTrue;
        [FormerlySerializedAs("invokeIfFalse")] public ObjectStateConditionalEvent InvokeIfFalse;

        public void Raise()
        {
            if (ObjectWithDesiredState.activeInHierarchy)
            {
                InvokeIfTrue.Invoke();
            }
            else
            {
                InvokeIfFalse.Invoke();
            }
        }
    }
}
