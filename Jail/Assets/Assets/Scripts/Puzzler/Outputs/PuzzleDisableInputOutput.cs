using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleDisableInputOutput : PuzzleBaseOutput
    {
        [SerializeField]
        PuzzleBaseInput inputToDisable;

        public override void OnInputsTriggered()
        {
            print("Enabling input!");
            inputToDisable.DisableInput(false);
        }

        public override void OnInputsUnTriggered()
        {
            print("Disabling input!");
            inputToDisable.DisableInput(true);
        }
    }
}
