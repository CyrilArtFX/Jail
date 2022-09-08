﻿using UnityEngine;
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

        bool savedTriggerState;

        new Collider collider = default;

        [SerializeField]
        protected new Renderer renderer;

        [Header("Materials")]
        [SerializeField]
        protected Material defaultMaterial = default;
        [SerializeField]
        Material disabledMaterial = default;

        void Awake()
        {
            SaveState();
            collider = GetComponent<Collider>();
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
            collider.enabled = !disable;
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
        }

        public void ResetState()
        {
            IsTriggered = savedTriggerState;
        }
    }
}
