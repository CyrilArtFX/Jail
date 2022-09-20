using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemPlus : MonoBehaviour
{
    EventSystem eventSystem = default;
    GameObject selectedButton;

    bool isControllerMode;
    Vector3 previousMousePos;

    [SerializeField]
    GameObject firstSelectedButton;

    void Awake()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Start()
    {
        selectedButton = firstSelectedButton;
        isControllerMode = true;

        previousMousePos = Vector3.zero;
    }

    void Update()
    {
        if (isControllerMode)
        {
            if (Input.mousePosition != previousMousePos || Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (eventSystem.currentSelectedGameObject != null)
                {
                    selectedButton = eventSystem.currentSelectedGameObject;
                }
                eventSystem.SetSelectedGameObject(null);
                isControllerMode = false;
            }
        }
        else
        {
            if (Input.GetAxis("ControllerX") != 0.0f || Input.GetAxis("ControllerY") != 0.0f)
            {
                eventSystem.SetSelectedGameObject(selectedButton);
                isControllerMode = true;
            }
        }

        previousMousePos = Input.mousePosition;
    }

    public void ForceControllerMode()
    {
        eventSystem.SetSelectedGameObject(selectedButton);
        isControllerMode = true;
    }

    public void ForceNoControllerMode()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            selectedButton = eventSystem.currentSelectedGameObject;
        }
        eventSystem.SetSelectedGameObject(null);
        isControllerMode = false;
    }
}
