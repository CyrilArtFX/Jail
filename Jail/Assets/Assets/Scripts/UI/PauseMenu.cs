using UnityEngine;
using System.Collections;
using Jail.Utility;
using UnityEngine.Rendering;
using Jail.Speedrun;

namespace Jail.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject pauseMenuObject;
        [SerializeField]
        EventSystemPlus eventSystemPlus;
        [SerializeField]
        Volume volume;

        bool pause;
        bool isInputDisabled = false;

        void Awake()
        {
            pauseMenuObject.SetActive(false);
            pause = false;
            Time.timeScale = 1.0f;
        }

        void Update()
        {
            volume.weight = Mathf.Lerp(volume.weight, pause ? 1.0f : 0.0f, Time.unscaledDeltaTime * 7.0f);

            //  get input
            bool is_pausing = false, is_controller = false;
            if (Input.GetButtonDown("PauseKeyboard"))
            {
                is_pausing = true;
            }
            else if (Input.GetButtonDown("PauseController"))
            {
                is_pausing = true;
                is_controller = true;
            }

            //  check for input
            if (!is_pausing) return;

            //  resume if paused
            if (pause)
            {
                ResumeGame();
            }
            //  pause otherwise
            else
            {
                PauseGame(is_controller);
            }
        }

        void PauseGame(bool activatedByController)
        {
            Time.timeScale = 0.0f;
            Player.instance.disableCommands = true;
            pauseMenuObject.SetActive(true);
            eventSystemPlus.ForceNoControllerMode();
            if (activatedByController)
            {
                eventSystemPlus.ForceControllerMode();
            }
            pause = true;
        }

        public void ResumeGame()
        {
            if (isInputDisabled) return;

            StartCoroutine(TimedResumeGame());
            isInputDisabled = true;
        }

        IEnumerator TimedResumeGame()
        {
            yield return new WaitForSecondsRealtime(0.0f);
            pauseMenuObject.SetActive(false);
            eventSystemPlus.ForceNoControllerMode();
            Player.instance.disableCommands = false;
            Time.timeScale = 1.0f;

            pause = false;
            isInputDisabled = false;
        }

        public void MainMenu()
        {
            if (isInputDisabled) return;

            Time.timeScale = 1.0f;
            SceneSwitcher.SwitchScene("MainMenu");
            isInputDisabled = true;

            //  speedrun: end run
            if (Speedrunner.instance != null)
            {
                Speedrunner.instance.EndRun(false);
            }
        }
    }
}
