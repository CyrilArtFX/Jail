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
        float dissolveValue = 0f;

        void Awake()
        {
            RetrieveAllRenderersOfChilds(this.transform);
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

                if (dissolveValue >= 1f)
                {
                    dissolveValue = 1f;
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

                if (dissolveValue <= 0f)
                {
                    dissolveValue = 0f;
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
            dissolveValue = 0f;
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat("Vector1_7d1cd5e28bfe440ea3a8058edcc3457f", dissolveValue);
            }
        }

        void RetrieveAllRenderersOfChilds(Transform object_to_check)
        {
            if (object_to_check.gameObject.TryGetComponent(out Renderer new_renderer))
            {
                renderers.Add(new_renderer);
            }

            if (object_to_check.childCount == 0) return;

            for (int i = 0; i < object_to_check.childCount; i++)
            {
                RetrieveAllRenderersOfChilds(object_to_check.GetChild(i));
            }
        }
    }
}
