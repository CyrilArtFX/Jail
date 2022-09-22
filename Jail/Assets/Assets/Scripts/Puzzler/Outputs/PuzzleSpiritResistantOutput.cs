using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleSpiritResistantOutput : PuzzleBaseOutput
    {
        ParticleSystem particles;

        BoxCollider boxCollider = default;

        protected override void Awake()
        {
            base.Awake();
            particles = GetComponentInChildren<ParticleSystem>();
            boxCollider = GetComponent<BoxCollider>();
            ChangeBoxTrigger(false);
        }

        public override void OnInputsTriggered()
        {
            print("Breaking wall!");
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            ChangeBoxTrigger(true);
        }

        public override void OnInputsUnTriggered()
        {
            print("Repairing wall!");
            particles.Play();
            ChangeBoxTrigger(false);
        }

        public void ChangeBoxTrigger(bool trigger)
        {
            boxCollider.isTrigger = trigger;
        }
    }
}