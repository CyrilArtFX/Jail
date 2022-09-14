using UnityEngine;
using System.Collections.Generic;
using Jail.Puzzler.Outputs;
using Jail.SavedObjects;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleBaseInput : MonoBehaviour, ICheckpointSaver
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

        bool savedTriggerState;
        bool savedEnabledState;

        Collider _collider;

        [SerializeField]
        protected bool startTriggered = false;

        [SerializeField]
        protected new Renderer renderer;

        [Header("Materials")]
        [SerializeField]
        protected Material defaultMaterial = default;
        [SerializeField]
        Material disabledMaterial = default;

        void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public virtual void Start()
        {
            IsTriggered = startTriggered;
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

        public virtual void DisableInput(bool disable)
        {
            _collider.enabled = !disable;
            enabled = !disable;

            if (disable)
            {
                IsTriggered = false;
            }

            //  change material depending on disable state
            renderer.material = disable ? disabledMaterial : defaultMaterial;
        }

        public void SaveState()
        {
            savedTriggerState = IsTriggered;
            savedEnabledState = enabled;
        }

        public void RestoreState()
        {
            if (savedEnabledState)
            {
                IsTriggered = savedTriggerState;
            }
        }
    }
}
