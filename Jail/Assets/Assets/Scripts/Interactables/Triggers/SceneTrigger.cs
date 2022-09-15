using UnityEngine;

using Jail.Utility;
using UnityEngine.SceneManagement;

namespace Jail.Interactables.Triggers
{
    public class SceneTrigger : BaseTrigger
    {
        [Header("Scene"), Scene, SerializeField]
        string sceneName;
        [SerializeField]
        bool useBlackFade = true;

        protected override void Awake()
        {
            base.Awake();

            color = Color.green;
        }

        protected override void OnTrigger(Collider other, bool is_enter)
        {
            if (is_enter)
            {
                if (useBlackFade)
                {
                    BlackFade.instance.eventEndOfFadeIn.AddListener(Switch);
                    BlackFade.instance.StartFade(FadeType.BothFadesWithRestore);
                }
                else
                {
                    Switch();
                }
            }
        }

        void Switch()
        {
            SceneSwitcher.SwitchScene(sceneName);
        }
    }
}