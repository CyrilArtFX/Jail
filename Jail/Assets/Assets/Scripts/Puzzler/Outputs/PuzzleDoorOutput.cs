using Jail.Puzzler.Inputs;
using UnityEngine;

namespace Jail.Puzzler.Outputs
{
    public class PuzzleDoorOutput : PuzzleBaseOutput
    {
        [SerializeField]
        TwoStatesAnim statesAnim;

        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip clip;

        public override void OnInputsTriggered()
        {
            print("Opening door!");
            statesAnim.ChangeBool(true);
            audioSource.PlayOneShot(clip);
        }

        public override void OnInputsUnTriggered()
        {
            print("Closing door!");
            statesAnim.ChangeBool(false);
            audioSource.PlayOneShot(clip);
        }
    }
}
