using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TutorialZone : MonoBehaviour
{
    [SerializeField] LayerMask detectedLayers = -1;
    [SerializeField, TextArea] string messageToDisplay;
    [SerializeField] TextMeshProUGUI textToDisplay;

    List<Collider> colliders = new List<Collider>();

    void Awake()
    {
        enabled = false;
        textToDisplay.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            Collider collider = colliders[i];
            if (!collider || !collider.gameObject.activeInHierarchy)
            {
                colliders.RemoveAt(i--);
                if (colliders.Count == 0)
                {
                    Exit();
                    enabled = false;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((detectedLayers & (1 << other.gameObject.layer)) != 0)
        {
            if (colliders.Count == 0)
            {
                Enter();
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
                Exit();
                enabled = false;
            }
        }
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (enabled && gameObject.activeInHierarchy)
        {
            return;
        }
#endif
        if (colliders.Count > 0)
        {
            colliders.Clear();
            Exit();
        }
    }

    void Enter()
    {
        textToDisplay.gameObject.SetActive(true);
        textToDisplay.text = messageToDisplay;
    }

    void Exit()
    {
        textToDisplay.gameObject.SetActive(false);
    }
}
