using UnityEngine;
using Jail.Utility;

namespace Jail.UI
{
    public class MainMenu : MonoBehaviour
    {
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
            SceneSwitcher.SwitchScene(1);
        }

        public void ShowOptions()
        {
            SceneSwitcher.SwitchScene(2);
        }

        public void HideOptions()
        {
            SceneSwitcher.SwitchScene(0);
        }
    }
}
