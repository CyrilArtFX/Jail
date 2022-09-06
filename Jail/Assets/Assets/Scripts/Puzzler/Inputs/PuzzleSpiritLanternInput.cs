using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleSpiritLanternInput : PuzzleBaseInput
    {
        [Header("Spirit Lantern"), Tooltip("How much time should it wait before turning itself off after trigger?"), SerializeField]
        float unTriggerTime = 20.0f;
        [SerializeField]
        LayerMask detectionMask;

        //  TODO: replace w/ final assets
        [Header("PREVIEW PLACEHOLDER"), SerializeField]
        Material triggerMaterial;
        Material defaultMaterial;
        [SerializeField]
        Renderer renderer;

        void Awake()
        {
            defaultMaterial = renderer.material;
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsTriggered) return;

            //  trigger
            IsTriggered = true;

            //  untrigger after time
            StartCoroutine(CoroutineExitTrigger());
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);

            //  change material depending on state
            renderer.material = state ? triggerMaterial : defaultMaterial;
        }

        IEnumerator CoroutineExitTrigger()
        {
            yield return new WaitForSeconds(unTriggerTime);

            IsTriggered = false;
        }
    }
}
