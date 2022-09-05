using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] 
    LayerMask detectedLayers = -1;
    [SerializeField] 
    UnityEvent onFirstEnter = default, onLastExit = default;
    List<Collider> colliders = new List<Collider>();

    [SerializeField] 
    float timeToReset = 0f;

     bool block = false;

    void Awake()
    {
        enabled = false;
    }

    void FixedUpdate()
    {
        for(int i = 0; i < colliders.Count; i++)
        {
            Collider collider = colliders[i];
            if(!collider || !collider.gameObject.activeInHierarchy)
            {
                colliders.RemoveAt(i--);
                if (colliders.Count == 0)
                {
                    StartCoroutine(TimerToLastExit());
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if((detectedLayers & (1 << other.gameObject.layer)) != 0)
        {
            if (colliders.Count == 0)
            {
                StopAllCoroutines();
                onFirstEnter.Invoke();
                enabled = true;
            }
            colliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((detectedLayers & (1 << other.gameObject.layer)) != 0)
        {
            if (colliders.Remove(other) && colliders.Count == 0)
            {
                StartCoroutine(TimerToLastExit());
            }
        }
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if(enabled && gameObject.activeInHierarchy)
        {
            return;
        }
#endif
        if(colliders.Count > 0)
        {
            colliders.Clear();
            onLastExit.Invoke();
        }
    }

    IEnumerator TimerToLastExit()
    {
        yield return new WaitForSeconds(timeToReset);
        yield return new WaitUntil(() => !block);
        onLastExit.Invoke();
        enabled = false;
    }

    public void Block(bool value)
    {
        block = value;
    }
}
