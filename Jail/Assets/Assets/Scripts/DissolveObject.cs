using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Jail
{
    public class DissolveObject : MonoBehaviour
    {
        List<Renderer> renderers = new List<Renderer>();

        [SerializeField]
        float dissolveTime;

        [SerializeField]
        UnityEvent passDissolve = default, passNoDissolve = default;

        bool dissolveStarted = false;
        bool inverseDissolveStarted = false;
        float dissolveValue = 0.0f;

        void Awake()
        {
            RetrieveAllRenderers();
        }

        void Update()
        {
            if (dissolveStarted)
            {
                dissolveValue += Time.deltaTime / dissolveTime;
                foreach (Renderer renderer in renderers)
                {
                    renderer.material.SetFloat("Vector1_7d1cd5e28bfe440ea3a8058edcc3457f", dissolveValue);
                }

                if (dissolveValue >= 1.0f)
                {
                    dissolveValue = 1.0f;
                    passDissolve.Invoke();
                    dissolveStarted = false;
                }
            }

            if (inverseDissolveStarted)
            {
                dissolveValue -= Time.deltaTime / dissolveTime;
                foreach (Renderer renderer in renderers)
                {
                    renderer.material.SetFloat("Vector1_7d1cd5e28bfe440ea3a8058edcc3457f", dissolveValue);
                }

                if (dissolveValue <= 0.0f)
                {
                    dissolveValue = 0.0f;
                    passNoDissolve.Invoke();
                    inverseDissolveStarted = false;
                }
            }
        }

        public void Dissolve()
        {
            dissolveStarted = true;
            inverseDissolveStarted = false;
        }

        public void InverseDissolve()
        {
            inverseDissolveStarted = true;
            dissolveStarted = false;
        }

        public void ForceNoDissolve()
        {
            dissolveStarted = false;
            inverseDissolveStarted = false;
            dissolveValue = 0.0f;
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat("Vector1_7d1cd5e28bfe440ea3a8058edcc3457f", dissolveValue);
            }
        }

        void RetrieveAllRenderers()
        {
            Renderer[] new_renderers = transform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in new_renderers)
            {
                renderers.Add(renderer);
            }
        }
    }
}

