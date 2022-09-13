using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jail.Utility
{
    public static class SceneSwitcher
    {
        public static void SwitchScene(int scene_id)
        {
            SceneManager.LoadScene(scene_id);
        }
    }
}
