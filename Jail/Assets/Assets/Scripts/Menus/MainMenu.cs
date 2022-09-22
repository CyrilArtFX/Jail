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
        float timeToggleInput = 1.0f;

        [SerializeField]
        LayerMask uiMask;

        bool isController = false;
        bool isInputDisabled = false;
        GlyphTMPButton hoveredButton;

        Vector3 farWorldPos, nearWorldPos, hitPos;

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

        }

        void Update()
        {
            if (isInputDisabled) return;

            if (isController)
            {
                ControllerUpdate();
            }
            else
            {
                MouseUpdate();
            }
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

            StartCoroutine(CoroutineDisableInputFor(timeToggleInput));
        }

        public void ShowMenu()
        {
            mainVC.Priority = 11;
            optionsVC.Priority = 10;

            StartCoroutine(CoroutineDisableInputFor(timeToggleInput));
        }

        IEnumerator CoroutineDisableInputFor(float time)
        {
            isInputDisabled = true;

            yield return new WaitForSeconds(time);

            isInputDisabled = false;
        }
    }
}
