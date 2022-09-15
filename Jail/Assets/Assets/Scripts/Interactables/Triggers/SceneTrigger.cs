using UnityEngine;

using Jail.Utility;
using UnityEngine.SceneManagement;

namespace Jail.Interactables.Triggers
{
    public class SceneTrigger : BaseTrigger
    {
        [Header("Scene"), Scene, SerializeField]
        string sceneName;

        protected override void Awake()
        {
            base.Awake();

            color = Color.green;
        }

        protected override void OnTrigger(Collider other, bool is_enter)
        {
            SceneSwitcher.SwitchScene(sceneName);
        }
    }
}