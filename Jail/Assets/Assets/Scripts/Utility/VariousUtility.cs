using System;
using UnityEngine.SceneManagement;

using TMPro;

namespace Jail.Utility
{
    public static class VariousUtility
    {
        public static int GetSceneBuildIDByName(string name)
        {
            for (int id = 0; id < SceneManager.sceneCountInBuildSettings; id++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(id);
                if (path.IndexOf(name) >= 0)
                {
                    return id;
                }
            }

            return -1;
        }

        public static void SetTMPTime(TMP_Text tmp, double time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            tmp.text = string.Format("{0:00}:{1:00}:{2:000}", span.Minutes, span.Seconds, span.Milliseconds);
        }
    }
}