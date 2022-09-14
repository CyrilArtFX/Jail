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

        public override void Start()
        {
            base.Start();
            if (!IsTriggered)
            {
                fireLight.TurnLightOff();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!Player.instance.IsSpirit || Player.instance.IsSpiritReturning) return;

            //  trigger
            IsTriggered = !IsTriggered;
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);

            if (state)
            {
                fireParticles.Play(true);
                if (!fireLight.lightOn)
                {
                    fireLight.FadeIn();
                }
            }
            else
            {
                fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (fireLight.lightOn)
                {
                    fireLight.FadeOut();
                }
            }
        }

        public override void DisableInput(bool disable)
        {
            base.DisableInput(disable);

            fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            if (fireLight.lightOn)
            {
                fireLight.TurnLightOff();
            }
        }
    }
}
