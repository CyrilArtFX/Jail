using System;
using UnityEngine;

using Jail.Utility;

namespace Jail.Interactables.Triggers
{
    [Flags]
    public enum BehaviourTriggerActions : byte
    {
        StartDisable = 1,
        DisableOnExit = 2,
    }

    public class BehaviourTrigger : BaseTrigger
    {
        [SerializeField]
        BehaviourTriggerActions actions;

        [SerializeField]
        Behaviour behaviour;

        void Start()
        {
            if (LayerMaskUtils.HasFlag((int) actions, (int) BehaviourTriggerActions.StartDisable))
            {
                behaviour.enabled = false;
            }
        }

        protected override void OnTrigger(Collider other, bool is_enter)
        {
            if (is_enter)
            {
                behaviour.enabled = true;
            }
            else
            {
                if (LayerMaskUtils.HasFlag((int) actions, (int) BehaviourTriggerActions.DisableOnExit))
                {
                    behaviour.enabled = false;
                }
            }
        }
    }
}