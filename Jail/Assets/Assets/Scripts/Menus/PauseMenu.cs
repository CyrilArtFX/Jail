using UnityEngine;
using System.Collections;
using Jail.Utility;

namespace Jail.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject pauseMenuObject;
        [SerializeField]
        EventSystemPlus eventSystemPlus;

        bool pause;

        private void Awake()
        {
            pauseMenuObject.SetActive(false);
            pause = false;
            Time.timeScale = 1.0f;
        }

        void Update()
        {
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
            StartCoroutine(TimedResumeGame());
        }

        IEnumerator TimedResumeGame()
        {
            yield return new WaitForSecondsRealtime(0.0f);
            pauseMenuObject.SetActive(false);
            pause = false;
            eventSystemPlus.ForceNoControllerMode();
            Player.instance.disableCommands = false;
            Time.timeScale = 1.0f;
        }

        public void MainMenu()
        {
            SceneSwitcher.SwitchScene("MainMenu");
        }
    }
}
