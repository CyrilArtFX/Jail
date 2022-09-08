using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail.Puzzler.Inputs;

namespace Jail.SavedObjects
{
    public class SavedPuzzleInput : SavedObject
    {
        bool savedTriggerState;
        bool savedEnabledState;

        PuzzleBaseInput puzzleInput;

        void Awake()
        {
            puzzleInput = gameObject.GetComponent<PuzzleBaseInput>();
        }


        public override void SaveState()
        {
            base.SaveState();

            savedTriggerState = puzzleInput.IsTriggered;
            savedEnabledState = puzzleInput.enabled;
        }

        public override void RestoreState()
        {
            base.RestoreState();

            if (savedEnabledState)
            {
                puzzleInput.IsTriggered = savedTriggerState;
            }
        }
    }
}
