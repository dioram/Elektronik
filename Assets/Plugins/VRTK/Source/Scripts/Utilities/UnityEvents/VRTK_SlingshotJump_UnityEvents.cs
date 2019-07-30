namespace VRTK.UnityEventHelper
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_SlingshotJump_UnityEvents")]
    public sealed class VRTK_SlingshotJump_UnityEvents : VRTK_UnityEvents<VRTK_SlingshotJump>
    {
        [Serializable]
        public sealed class SlingshotJumpEvent : UnityEvent<object> { }

        public SlingshotJumpEvent OnSlingshotJumped = new SlingshotJumpEvent();

        protected override void AddListeners(VRTK_SlingshotJump component)
        {
            component.SlingshotJumped += SlingshotJumped;
        }

        protected override void RemoveListeners(VRTK_SlingshotJump component)
        {
            component.SlingshotJumped -= SlingshotJumped;
        }

        private void SlingshotJumped(object o)
        {
            OnSlingshotJumped.Invoke(o);
        }
    }
}