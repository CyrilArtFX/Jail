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
        private AudioClip clipOn;
        [SerializeField]
        private AudioClip clipOff;


        public override void OnInputsTriggered()
        {
            print("Opening door!");
            statesAnim.ChangeBool(true);
            audioSource.PlayOneShot(clipOn);
        }

        public override void OnInputsUnTriggered()
        {
            print("Closing door!");
            statesAnim.ChangeBool(false);
            audioSource.PlayOneShot(clipOff);
        }
    }
}
