using UnityEngine;
using UnityEngine.SceneManagement;

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
    }
}