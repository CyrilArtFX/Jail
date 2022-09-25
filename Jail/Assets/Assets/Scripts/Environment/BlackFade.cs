using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Jail.Speedrun;

namespace Jail
{
    enum FadeStates
    {
        FadeIn,
        StayBlack,
        FadeOut,
        Off
    }

    public enum FadeType
    {
        OnlyFadeIn,
        OnlyFadeOut,
        BothFadesWithRestore
    }


    public class BlackFade : MonoBehaviour
    {
        [Header("Black Fade")]
        [SerializeField]
        Image blackFadeImage = default;
        [SerializeField]
        float blackFadeHalfTime = 1.0f, fullBlackTime = 0.5f;
        [SerializeField]
        AnimationCurve blackFadeCurve = default;

        float timeSinceBlackFadeStarted = 0.0f;

        [SerializeField]
        bool fadeInAwake = true;
        FadeStates blackFadeState = FadeStates.Off;
        [SerializeField]
        FadeType blackFadeType = FadeType.OnlyFadeOut;

        [SerializeField]
        bool shouldToggleCommands = true;

        [HideInInspector]
        public UnityEvent eventEndOfFadeIn = default;

        public static BlackFade instance;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //  start scene-defined fade
            if (fadeInAwake)
            {
                StartFade(blackFadeType, shouldToggleCommands);
            }
        }

        void Update()
        {
            switch (blackFadeState)
            {
                case FadeStates.Off:
                    return;

                case FadeStates.FadeIn:

                    timeSinceBlackFadeStarted += Time.unscaledDeltaTime;
                    if (timeSinceBlackFadeStarted >= blackFadeHalfTime)
                    {
                        timeSinceBlackFadeStarted = 0.0f; 


                        blackFadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                        if (blackFadeType == FadeType.BothFadesWithRestore)
                        {
                            blackFadeState = FadeStates.StayBlack;

                            eventEndOfFadeIn.Invoke();
                        }
                        else
                        {
                            blackFadeState = FadeStates.Off;
                        }
                    }
                    else
                    {
                        float transition_fraction = timeSinceBlackFadeStarted / blackFadeHalfTime;
                        blackFadeImage.color = new Color(0.0f, 0.0f, 0.0f, blackFadeCurve.Evaluate(transition_fraction));
                    }

                    break;

                case FadeStates.StayBlack:

                    timeSinceBlackFadeStarted += Time.unscaledDeltaTime;
                    if (timeSinceBlackFadeStarted >= fullBlackTime)
                    {
                        timeSinceBlackFadeStarted = 0.0f;
                        blackFadeState = FadeStates.FadeOut;
                    }

                    break;

                case FadeStates.FadeOut:

                    timeSinceBlackFadeStarted += Time.unscaledDeltaTime;
                    if (timeSinceBlackFadeStarted >= blackFadeHalfTime)
                    {
                        timeSinceBlackFadeStarted = 0.0f;
                        blackFadeState = FadeStates.Off;
                        blackFadeImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

                        if (shouldToggleCommands)
                        {
                            Player.instance.disableCommands = false;

                            //  resume speedrun
                            if (Speedrunner.instance != null)
							{
                                if (!Speedrunner.instance.HasStarted)
					            {
                                    Speedrunner.instance.StartRun();
					            }
                                else
								{
                                    Speedrunner.instance.SetPause(false);
								}
							}
                        }
                    }
                    else
                    {
                        float transition_fraction = 1 - (timeSinceBlackFadeStarted / blackFadeHalfTime);
                        blackFadeImage.color = new Color(0.0f, 0.0f, 0.0f, blackFadeCurve.Evaluate(transition_fraction));
                    }

                    break;
            }
        }

        public void StartFade(FadeType fadeType, bool toggle_commands = true)
        {
            shouldToggleCommands = toggle_commands;
            if (shouldToggleCommands)
            {
                //  pause speedrun
                if (Speedrunner.instance != null)
				{
                    Speedrunner.instance.SetPause(true);
				} 

                Player.instance.disableCommands = true;
            }

            timeSinceBlackFadeStarted = 0.0f;
            blackFadeType = fadeType;
            if (fadeType == FadeType.OnlyFadeOut)
            {
                blackFadeState = FadeStates.FadeOut;
                blackFadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            }
            else
            {
                blackFadeState = FadeStates.FadeIn;
                blackFadeImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }
    }
}
