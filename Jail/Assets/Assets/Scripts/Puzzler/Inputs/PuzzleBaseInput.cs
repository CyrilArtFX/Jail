using UnityEngine;
using System.Collections.Generic;
using Jail.Puzzler.Outputs;
using Jail.SavedObjects;

namespace Jail.Puzzler.Inputs
{
    public class PuzzleBaseInput : MonoBehaviour, ICheckpointSaver
    {
        protected readonly List<PuzzleBaseOutput> outputs = new List<PuzzleBaseOutput>();

        bool isRawTriggered = false;

        public bool IsRawTriggered
        {
            get => isRawTriggered;
            protected set
            {
                isRawTriggered = value;
                OnTrigger(value);
            }
        }

        public bool IsTriggered => isReversed ? !isRawTriggered : isRawTriggered;

        bool savedTriggerState;
        bool savedEnabledState;

        Collider _collider;

        [SerializeField]
        protected bool startTriggered = false;
        [SerializeField]
        protected bool isReversed = false;

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
            IsRawTriggered = startTriggered;
            SaveState();
        }

        public void LinkOutput(PuzzleBaseOutput output)
        {
            outputs.Add(output);
        }

        protected virtual void OnTrigger(bool state)
        {
            print("input: " + this + " state is " + isRawTriggered);

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
                IsRawTriggered = false;
            }

            //  change material depending on disable state
            renderer.material = disable ? disabledMaterial : defaultMaterial;
        }

        public void SaveState()
        {
            savedTriggerState = isRawTriggered;
            savedEnabledState = enabled;
        }

        public void RestoreState()
        {
            if (savedEnabledState)
            {
                IsRawTriggered = savedTriggerState;
            }
        }
    }
}
