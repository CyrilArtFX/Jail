using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail.LightControl;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleSpiritLanternInput : PuzzleBaseInput
    {
        [SerializeField]
        LayerMask detectionMask;
        [SerializeField]
        ParticleSystem fireParticles;
        [SerializeField]
        LightController fireLight;

        //  TODO: replace w/ final assets
        [Header("PREVIEW PLACEHOLDER"), SerializeField]
        Material triggerMaterial;

        protected override void Awake()
        {
            base.Awake();
            fireLight.TurnLightOff();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!Player.instance.IsSpirit) return;

            //  trigger
            IsTriggered = !IsTriggered;
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);

            //  change material depending on state
            renderer.material = state ? triggerMaterial : defaultMaterial;

            if (state)
            {
                fireParticles.Play(true);
                fireLight.FadeIn();
            }
            else
            {
                fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                fireLight.FadeOut();
            }
        }

        public override void DisableInput(bool disable)
        {
            base.DisableInput(disable);

            fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            fireLight.TurnLightOff();
        }
    }
}
