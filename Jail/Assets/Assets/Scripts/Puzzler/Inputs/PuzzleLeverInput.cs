using UnityEngine;
using System.Collections;
using Jail.Utility;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleLeverInput : PuzzleBaseInput
    {
        [Header("Lever"), SerializeField]
        LayerMask detectionMask;
        bool isPlayerInFront = false;
        [SerializeField]
        TwoStatesAnim statesAnim;

        void Update()
        {
            if (!isPlayerInFront) return;
            if (Player.instance.IsSpirit) return;
            if (Player.instance.AttachedCrate != null) return;

            if (Input.GetButtonDown("Interact"))
            {
                IsRawTriggered = !IsRawTriggered;
                Player.instance.AnimTriggerLever();
            }
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);
            statesAnim.ChangeBool(state);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;

            isPlayerInFront = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;

            isPlayerInFront = false;
        }
    }
}
