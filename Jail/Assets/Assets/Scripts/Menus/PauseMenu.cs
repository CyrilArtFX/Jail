using UnityEngine;
using Jail.Utility;

namespace Jail.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject pauseMenuObject;

        bool pause;

        private void Awake()
        {
            pauseMenuObject.SetActive(false);
            pause = false;
            Time.timeScale = 1f;
        }

        void Update()
        {
            if (Input.GetButtonDown("Pause"))
            {
                if (pause)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        void PauseGame()
        {
            Time.timeScale = 0.0f;
            pauseMenuObject.SetActive(true);
            pause = true;
        }

        public void ResumeGame()
        {
            pauseMenuObject.SetActive(false);
            pause = false;
            Time.timeScale = 1.0f;
        }

        public void MainMenu()
        {
            SceneSwitcher.SwitchScene(0);
        }
    }
}
