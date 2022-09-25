using UnityEngine;

using Jail.Utility;
using UnityEngine.SceneManagement;

using Jail.Speedrun;

namespace Jail.Interactables.Triggers
{
    public class SceneTrigger : BaseTrigger
    {
        [Header("Scene"), Scene, SerializeField]
        string sceneName;

		[SerializeField]
        bool shouldEndSpeedrun = false;

        protected override void Awake()
        {
            base.Awake();

            color = Color.green;
        }

        protected override void OnTrigger(Collider other, bool is_enter)
        {
            if (is_enter)
            {
                //  end speedrun
                if (Speedrunner.instance != null)
				{
                    if (shouldEndSpeedrun)
					{
                        //  end run
                        Speedrunner.instance.EndRun();
					}
                    else
					{
                        //  split level time
                        Speedrunner.instance.SplitLevelTime();
					}
				}

                //  switch scene
                SceneSwitcher.SwitchScene(sceneName);
            }
        }
    }
}