using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        FullFadeWithRestore
    }


    public class BlackFade : MonoBehaviour
    {
        [Header("Black Fade")]
        [SerializeField]
        Image blackFadeImage = default;
        [SerializeField]
        float blackFadeHalfTime = 1f, fullBlackTime = 0.5f;
        [SerializeField]
        AnimationCurve blackFadeCurve = default;

        float timeSinceBlackFadeStarted = 0f;
        FadeStates blackFadeState = FadeStates.Off;
        FadeType blackFadeType = FadeType.OnlyFadeIn;

        public static BlackFade instance;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if (blackFadeState != FadeStates.Off)
            {
                if (blackFadeState == FadeStates.FadeIn)
                {
                    timeSinceBlackFadeStarted += Time.deltaTime;
                    if (timeSinceBlackFadeStarted >= blackFadeHalfTime)
                    {
                        timeSinceBlackFadeStarted = 0f;
                        blackFadeImage.color = new Color(0, 0, 0, 1);
                        if(blackFadeType == FadeType.FullFadeWithRestore)
                        {
                            blackFadeState = FadeStates.StayBlack;

                            GlobalCheckpointAndRespawn.instance.RestoreCheckpoint();
                        }
                        else
                        {
                            blackFadeState = FadeStates.Off;
                        }
                    }
                    else
                    {
                        float transition_fraction = timeSinceBlackFadeStarted / blackFadeHalfTime;
                        blackFadeImage.color = new Color(0, 0, 0, blackFadeCurve.Evaluate(transition_fraction));
                    }
                }
                else if (blackFadeState == FadeStates.StayBlack)
                {
                    timeSinceBlackFadeStarted += Time.deltaTime;
                    if (timeSinceBlackFadeStarted >= fullBlackTime)
                    {
                        timeSinceBlackFadeStarted = 0f;
                        blackFadeState = FadeStates.FadeOut;
                    }
                }
                else if (blackFadeState == FadeStates.FadeOut)
                {
                    timeSinceBlackFadeStarted += Time.deltaTime;
                    if (timeSinceBlackFadeStarted >= blackFadeHalfTime)
                    {
                        timeSinceBlackFadeStarted = 0f;
                        blackFadeState = FadeStates.Off;
                        blackFadeImage.color = new Color(0, 0, 0, 0);
                        Player.instance.dead = false;
                    }
                    else
                    {
                        float transition_fraction = 1 - (timeSinceBlackFadeStarted / blackFadeHalfTime);
                        blackFadeImage.color = new Color(0, 0, 0, blackFadeCurve.Evaluate(transition_fraction));
                    }
                }
            }
        }

        public void StartFade(FadeType fadeType)
        {
            Player.instance.dead = true;
            timeSinceBlackFadeStarted = 0f;
            blackFadeType = fadeType;
            if(fadeType == FadeType.OnlyFadeOut)
            {
                blackFadeState = FadeStates.FadeOut;
            }
            else
            {
                blackFadeState = FadeStates.FadeIn;
            }
        }
    }
}
