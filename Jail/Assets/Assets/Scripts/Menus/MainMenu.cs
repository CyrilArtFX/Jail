using UnityEngine;
using Jail.Utility;
using TMPro;

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
    }
}
