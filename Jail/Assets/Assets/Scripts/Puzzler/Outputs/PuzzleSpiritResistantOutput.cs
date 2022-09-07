using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleSpiritResistantOutput : PuzzleBaseOutput
    {
        [SerializeField]
        DissolveObject dissolve;

        BoxCollider boxCollider = default;

        protected override void Awake()
        {
            base.Awake();
            boxCollider = GetComponent<BoxCollider>();
        }

        public override void OnInputsTriggered()
        {
            print("Breaking wall!");
            dissolve.Dissolve();
        }

        public override void OnInputsUnTriggered()
        {
            print("Repairing wall!");
            dissolve.InverseDissolve();
        }

        public void ChangeBoxTrigger(bool trigger)
        {
            boxCollider.isTrigger = trigger;
        }
    }
}