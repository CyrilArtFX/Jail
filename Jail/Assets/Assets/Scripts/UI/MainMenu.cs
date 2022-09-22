using UnityEngine;
using Jail.Utility;
using Jail.Environment.Glyphs;
using Cinemachine;
using System.Collections;

namespace Jail.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Scene"), Scene, SerializeField]
        string playScene;

        [SerializeField]
        CinemachineVirtualCamera mainVC, optionsVC;

        [SerializeField]
        float timeToggleInput = 1.0f, timeForControllerPress = 0.2f;

        [SerializeField]
        LayerMask uiMask;

        [SerializeField]
        MenuController mainMenu, optionsMenu;

        MenuController currentMenu;

        bool isMouseControlled = false;
        bool isInputDisabled = false;
        GlyphTMPButton hoveredButton;

        Vector3 farWorldPos, nearWorldPos, hitPos;

        Vector3 lastMousePos = Vector3.zero;

        void Start()
        {
            currentMenu = mainMenu;    
        }

        public void QuitGame()
        {
            BlackFade.instance.eventEndOfFadeIn.AddListener(
                () =>
                {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                }
            );
            BlackFade.instance.StartFade(FadeType.BothFadesWithRestore);
        }

        void MouseUpdate()
        {
            //  handle button behaviour w/ mouse
            Camera camera = Camera.main;

            //  get far & near mouse positions
            Vector3 far_mouse_pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.farClipPlane);
            Vector3 near_mouse_pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane);

            //  translate mouse position to world
            farWorldPos = camera.ScreenToWorldPoint(far_mouse_pos);
            nearWorldPos = camera.ScreenToWorldPoint(near_mouse_pos);

            //  check for button input
            if (Physics.Raycast(nearWorldPos, farWorldPos - nearWorldPos, out RaycastHit hit, 16.0f, uiMask))
            {
                GameObject hit_object = hit.collider.gameObject;

                if (hit_object.TryGetComponent(out GlyphTMPButton button))
                {
                    //  click button
                    if (Input.GetMouseButtonUp(0))
                    {
                        button.DoClick();
                        isInputDisabled = true;
                    }
                    else
                    {
                        //  unhover last button
                        if (hoveredButton != null)
                        {
                            hoveredButton.IsHovered = false;
                        }

                        //  set hovered
                        hoveredButton = button;
                        button.IsHovered = true;
                    }
                }

                hitPos = hit.point;
            }
            else
            {
                //  unselect hovered button
                if (hoveredButton != null)
                {
                    hoveredButton.IsHovered = false;
                    hoveredButton = null;
                }
            }
        }

        void ControllerUpdate()
        {
            float axis = Input.GetAxis("UpDown");

            //  move buttons
            if (axis < 0.0f)
            {
                currentMenu.NextButton();
                DisableInputFor(timeForControllerPress);
            }
            else if (axis > 0.0f)
            {
                currentMenu.PreviousButton();
                DisableInputFor(timeForControllerPress);
            }

            //  enter
            if (Input.GetButtonDown("Submit"))
            {
                if (currentMenu.CurrentButton != null)
                {
                    currentMenu.CurrentButton.DoClick();
                }
            }
        }

        void Update()
        {
            if (isInputDisabled) return;

            if (isMouseControlled)
            {
                //  check for controller input
                float axis = Input.GetAxis("UpDown");
                if (axis != 0.0f)
                {
                    isMouseControlled = false;
                    print("controller! " + axis);
                }
            }
            else
            {
                //  check for mouse movement
                Vector3 mouse_pos = Input.mousePosition;
                
                Vector3 mouse_delta = lastMousePos - mouse_pos;
                if (lastMousePos != Vector3.zero && mouse_delta.sqrMagnitude > 0)
                {
                    isMouseControlled = true;
                    print("mouse! " + mouse_delta.sqrMagnitude);
                }
                
                lastMousePos = mouse_pos;
            }

            if (!isMouseControlled)
            {
                ControllerUpdate();
            }
        
            MouseUpdate();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(nearWorldPos, hitPos);
            Gizmos.DrawSphere(hitPos, 0.1f);
        }

        public void Play()
        {
            SceneSwitcher.SwitchScene(playScene);
        }

        public void ShowOptions()
        {
            mainVC.Priority = 10;
            optionsVC.Priority = 11;
            currentMenu = optionsMenu;

            DisableInputFor(timeToggleInput);
        }

        public void ShowMenu()
        {
            mainVC.Priority = 11;
            optionsVC.Priority = 10;
            currentMenu = mainMenu;

            DisableInputFor(timeToggleInput);
        }

        IEnumerator CoroutineDisableInputFor(float time)
        {
            isInputDisabled = true;

            yield return new WaitForSeconds(time);

            isInputDisabled = false;
        }

        void DisableInputFor(float time)
        {
            StartCoroutine(CoroutineDisableInputFor(time));
        }
    }
}
