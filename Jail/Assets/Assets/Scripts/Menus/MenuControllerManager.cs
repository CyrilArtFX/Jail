using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jail.UI
{
    public class MenuControllerManager : MonoBehaviour
    {
        bool isControllerMode = false;
        Vector3 previousMousePos = Vector3.zero;

        MenuControllerButton currentButton;

        bool controllerDisabled = false;
        float controllerDisablingTime = 0.3f;

        [SerializeField]
        MenuControllerButton defaultCurrentButton = default;

        void Start()
        {
            currentButton = defaultCurrentButton;
            previousMousePos = Input.mousePosition;
        }

        void Update()
        {
            if (isControllerMode)
            {
                if (Input.mousePosition != previousMousePos)
                {
                    isControllerMode = false;
                    currentButton.GoNormal();
                }

                if (!controllerDisabled)
                {
                    if (Input.GetAxis("ControllerX") > 0.8f)
                    {
                        if (currentButton.buttonAtRight != null)
                        {
                            ChangeCurrentButton(currentButton.buttonAtRight);
                        }
                    }

                    if (Input.GetAxis("ControllerX") < -0.8f)
                    {
                        if (currentButton.buttonAtLeft != null)
                        {
                            ChangeCurrentButton(currentButton.buttonAtLeft);
                        }
                    }

                    if (Input.GetAxis("ControllerY") > 0.8f)
                    {
                        if (currentButton.buttonAtUp != null)
                        {
                            ChangeCurrentButton(currentButton.buttonAtUp);
                        }
                    }

                    if (Input.GetAxis("ControllerY") < -0.8f)
                    {
                        if (currentButton.buttonAtDown != null)
                        {
                            ChangeCurrentButton(currentButton.buttonAtDown);
                        }
                    }
                }

                if (Input.GetButtonDown("ControllerPress"))
                {
                    currentButton.Press();
                }
            }
            else
            {
                if (Input.GetAxis("ControllerX") != 0.0f || Input.GetAxis("ControllerY") != 0.0f)
                {
                    isControllerMode = true;
                    currentButton.GoHighlight();
                    StartCoroutine(DisableController());
                }
            }

            previousMousePos = Input.mousePosition;
        }

        void ChangeCurrentButton(MenuControllerButton newCurrentButton)
        {
            currentButton.GoNormal();
            currentButton = newCurrentButton;
            currentButton.GoHighlight();
            StartCoroutine(DisableController());
        }

        IEnumerator DisableController()
        {
            controllerDisabled = true;
            yield return new WaitForSeconds(controllerDisablingTime);
            controllerDisabled = false;
        }
    }

}