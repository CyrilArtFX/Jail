using UnityEngine;
using Jail.Utility;

namespace Jail.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Scene")]
        [Scene, SerializeField]
        string playScene;
        [Scene, SerializeField]
        string optionScene;
        [Scene, SerializeField]
        string menuScene;

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void Play()
        {
            SceneSwitcher.SwitchScene(playScene);
        }

        public void ShowOptions()
        {
            SceneSwitcher.SwitchScene(optionScene);
        }

        public void HideOptions()
        {
            SceneSwitcher.SwitchScene(menuScene);
        }
    }
}
