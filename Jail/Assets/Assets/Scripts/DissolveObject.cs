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
            GetAllRenderersOfChilds(this.transform);
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

        private void GetAllRenderersOfChilds(Transform objectToCheck)
        {
            if (objectToCheck.gameObject.TryGetComponent(out Renderer newRenderer))
            {
                renderers.Add(newRenderer);
            }

            if (objectToCheck.childCount == 0) return;

            for (int i = 0; i < objectToCheck.childCount; i++)
            {
                GetAllRenderersOfChilds(objectToCheck.GetChild(i));
            }
        }
    }
}

