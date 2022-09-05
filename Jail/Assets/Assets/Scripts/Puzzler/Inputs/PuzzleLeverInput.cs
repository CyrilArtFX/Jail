using UnityEngine;
using System.Collections;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleLeverInput : PuzzleBaseInput
    {
        bool isPlayerInFront = false;
        [SerializeField]
        TwoStatesAnim statesAnim;

        void Update()
        {
            if (!isPlayerInFront) return;

            if (Input.GetButtonDown("Interact"))
            {
                IsTriggered = !IsTriggered;
            }
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);
            statesAnim.ChangeBool(state);
        }

        void OnTriggerEnter(Collider other)
        {
            isPlayerInFront = true;
        }

        void OnTriggerExit(Collider other)
        {
            isPlayerInFront = false;
        }
    }
}
