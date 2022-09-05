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

        void Update()
        {
            if (!isPlayerInFront) return;

            if (Input.GetButtonDown("Interact"))
            {
                IsTriggered = !IsTriggered;
            }
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
