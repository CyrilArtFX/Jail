using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleSpiritResistantOutput : PuzzleBaseOutput
    {
        [SerializeField]
        DissolveObject dissolve;

        void FixedUpdate()
        {
            if(dissolve.Dissolving)
            {
                GetComponent<BoxCollider>().isTrigger = dissolve.IsDissolve;
            }
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
    }
}