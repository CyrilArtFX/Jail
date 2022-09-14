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
            SceneSwitcher.SwitchScene("SceneTest2D");
        }

        public void ShowOptions()
        {
            SceneSwitcher.SwitchScene("OptionsMenu");
        }

        public void HideOptions()
        {
            SceneSwitcher.SwitchScene("MainMenu");
        }
    }
}
