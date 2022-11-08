using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jail.Utility
{
    public static class SceneSwitcher
    {
        public static void SwitchScene(int scene_id)
        {
            if (BlackFade.instance != null)
            {
                BlackFade.instance.eventEndOfFadeIn.AddListener(
                    () => SceneManager.LoadScene(scene_id)
                );
                BlackFade.instance.StartFade(FadeType.BothFadesWithRestore);
            }
            else
            {
                SceneManager.LoadScene(scene_id);
            }

        }
        public static void SwitchScene(string scene_name)
        {
            SwitchScene(VariousUtility.GetSceneBuildIDByName(scene_name));
        }
    }
}
