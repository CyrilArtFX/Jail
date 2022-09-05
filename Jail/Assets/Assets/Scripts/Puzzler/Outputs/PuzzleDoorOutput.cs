using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleDoorOutput : PuzzleBaseOutput
    {
        [SerializeField]
        TwoStatesAnim statesAnim;

        public override void OnInputsTriggered()
        {
            print("Opening door!");
            statesAnim.ChangeBool(true);

        }

        public override void OnInputsUnTriggered()
        {
            print("Closing door!");
            statesAnim.ChangeBool(false);
        }
    }
}
