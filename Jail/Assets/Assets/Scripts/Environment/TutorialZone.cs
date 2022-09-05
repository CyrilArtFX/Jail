using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Jail.Utility;

public class TutorialZone : MonoBehaviour
{
    [SerializeField] 
    LayerMask detectedLayers = -1;
    [SerializeField, TextArea] 
    string messageToDisplay;
    [SerializeField] 
    TextMeshProUGUI textToDisplay;

    void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
            return;

        Enter();
    }

    void OnTriggerExit(Collider other)
    {
        if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
            return;

        Exit();
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
