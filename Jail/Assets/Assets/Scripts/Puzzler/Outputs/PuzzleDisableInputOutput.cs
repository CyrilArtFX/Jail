using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleDisableInputOutput : PuzzleBaseOutput
    {
        [SerializeField]
        PuzzleBaseInput inputToDisable = default;

        [SerializeField]
        bool startDisable = false;

        void Start()
        {
            if (startDisable)
            {
                inputToDisable.DisableInput(true);
                inputToDisable.Start();
            }
        }

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
