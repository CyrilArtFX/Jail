using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleSpiritResistantOutput : PuzzleBaseOutput
    {
        [SerializeField]
        DissolveObject dissolve;

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
            GetComponent<BoxCollider>().isTrigger = trigger;
        }
    }
}