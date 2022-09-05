using UnityEngine;
using System.Collections;
using Jail.Utility;

namespace Jail.Puzzler.Inputs
{
    public class PuzzlePressurePlateInput : PuzzleBaseInput
    {
        [Header("Pressure Plate"), Tooltip("How much time should it wait before turning the trigger off after exit?"), SerializeField]
        float exitTriggerTime = 0.0f;
        [SerializeField]
        LayerMask detectionMask;

        int activeCollidersCount = 0;
        Coroutine oldExitCoroutine;

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;


            //  remove old coroutine (prevent disabling itself when going out-&-in of the plate)
            if (oldExitCoroutine != null)
            {
                StopCoroutine(oldExitCoroutine);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;

        }

        IEnumerator CoroutineExitTrigger()
        {
            yield return new WaitForSeconds(exitTriggerTime);

            IsTriggered = false;
        }
    }
}
