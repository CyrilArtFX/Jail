using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemPlus : MonoBehaviour
{
    EventSystem eventSystem = default;
    GameObject selectedButton;

    void Awake()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Start()
    {
        selectedButton = eventSystem.currentSelectedGameObject;
    }

    void Update()
    {
    }
}
