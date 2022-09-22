using UnityEngine;

using Jail.LightControl;
using Jail.Utility;


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

        public AudioSource source;
        public AudioClip clip;

        public override void Start()
        {
            base.Start();
            if (!IsRawTriggered)
            {
                fireLight.TurnLightOff();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer)) return;
            if (!Player.instance.IsSpirit || Player.instance.IsSpiritReturning) return;

            //  trigger
            IsRawTriggered = !IsRawTriggered;
        }

        protected override void OnTrigger(bool state)
        {
            base.OnTrigger(state);

            if (state)
            {
                fireParticles.Play(true);
                source.PlayOneShot(clip);

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
