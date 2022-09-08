using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleSpiritLanternInput : PuzzleBaseInput
    {
        [SerializeField]
        LayerMask detectionMask;
        [SerializeField]
        ParticleSystem particles;

        //  TODO: replace w/ final assets
        [Header("PREVIEW PLACEHOLDER"), SerializeField]
        Material triggerMaterial;

        void OnTriggerEnter(Collider other)
        {
            if (!Player.instance.IsSpirit) return;

            //  trigger
            IsTriggered = !IsTriggered;
            if (IsTriggered)
            {
                particles.Play();
            }
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);

            //  change material depending on state
            renderer.material = state ? triggerMaterial : defaultMaterial;
        }
    }
}
