using UnityEngine;
using System.Collections.Generic;
using Jail.Puzzler.Outputs;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleBaseInput : MonoBehaviour
    {
        protected readonly List<PuzzleBaseOutput> outputs = new List<PuzzleBaseOutput>();

        bool isTriggered = false;

        [SerializeField]
        protected bool disabled = false;

        public bool IsTriggered
        {
            get => isTriggered;
            protected set
            {
                isTriggered = value;
                OnTrigger(value);
            }
        }

        [SerializeField]
        protected new Renderer renderer;

        [Header("Materials")]
        [SerializeField]
        protected Material defaultMaterial = default;
        [SerializeField]
        Material disabledMaterial = default;

        void Awake()
        {
            //  change material depending on disable state
            renderer.material = disabled ? disabledMaterial : defaultMaterial;
        }

        public void LinkOutput(PuzzleBaseOutput output)
        {
            outputs.Add(output);
        }

        protected virtual void OnTrigger(bool state)
        {
            if (disabled) return;

            print("input: " + this + " state is " + isTriggered);

            //  alert output from change
            foreach (PuzzleBaseOutput output in outputs)
            {
                output.AlertInputStateChange(this, IsTriggered);
            }
        }

        public void DisableInput(bool disable)
        {
            disabled = disable;
            //  change material depending on disable state
            renderer.material = disable ? disabledMaterial : defaultMaterial;
        }
    }
}
