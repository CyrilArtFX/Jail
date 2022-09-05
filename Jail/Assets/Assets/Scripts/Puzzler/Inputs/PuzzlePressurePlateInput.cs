﻿using UnityEngine;
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

        TwoStatesAnim statesAnim;

        int activeCollidersCount = 0;
        Coroutine oldExitCoroutine;

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;

            //  trigger if it is the first collider
            if ((activeCollidersCount++) == 0)
            {
                IsTriggered = true;
            }

            //  remove old coroutine (prevent disabling itself when going out-&-in of the plate)
            if (oldExitCoroutine != null)
            {
                StopCoroutine(oldExitCoroutine);
            }
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);
            statesAnim.ChangeBool(state);
        }

        void OnTriggerExit(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;

            //  untrigger if it is the last collider
            if ((--activeCollidersCount) == 0)
            {
                oldExitCoroutine = StartCoroutine(CoroutineExitTrigger());
            }
        }

        IEnumerator CoroutineExitTrigger()
        {
            yield return new WaitForSeconds(exitTriggerTime);

            IsTriggered = false;
        }
    }
}
