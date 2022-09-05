using UnityEngine;
using System.Collections.Generic;
using Jail.Puzzler.Outputs;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleBaseInput : MonoBehaviour
    {
        protected readonly List<PuzzleBaseOutput> outputs = new List<PuzzleBaseOutput>();

        bool isTriggered = false;
        public bool IsTriggered
        {
            get => isTriggered;
            protected set
            {
                isTriggered = value;
                OnTrigger(value);
            }
        }

        public void LinkOutput(PuzzleBaseOutput output)
        {
            outputs.Add(output);
        }

        protected virtual void OnTrigger(bool state)
        {
            print("input: " + this + " state is " + isTriggered);

            //  alert output from change
            foreach (PuzzleBaseOutput output in outputs)
            {
                output.AlertInputStateChange(this, IsTriggered);
            }
        }
    }
}
