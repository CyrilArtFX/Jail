using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jail
{
    enum TransitionStates
    {
        FadeIn,
        StayBlack,
        FadeOut,
        Off
    }


    public class BlackTransition : MonoBehaviour
    {
        [Header("Black Transition")]
        [SerializeField]
        Image blackTransitionImage = default;
        [SerializeField]
        float blackTransitionHalfTime = 1f, fullBlackTime = 0.5f;
        [SerializeField]
        AnimationCurve blackTransitionCurve = default;

        float timeSinceBlackTransitionStarted = 0f;
        TransitionStates blackTransitionState = TransitionStates.Off;

        public static BlackTransition instance;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if (blackTransitionState != TransitionStates.Off)
            {
                if (blackTransitionState == TransitionStates.FadeIn)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= blackTransitionHalfTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = TransitionStates.StayBlack;
                        blackTransitionImage.color = new Color(0, 0, 0, 1);

                        GlobalCheckpointAndRespawn.instance.RestoreCheckpoint();
                    }
                    else
                    {
                        float transition_fraction = timeSinceBlackTransitionStarted / blackTransitionHalfTime;
                        blackTransitionImage.color = new Color(0, 0, 0, blackTransitionCurve.Evaluate(transition_fraction));
                    }
                }
                else if (blackTransitionState == TransitionStates.StayBlack)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= fullBlackTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = TransitionStates.FadeOut;
                    }
                }
                else if (blackTransitionState == TransitionStates.FadeOut)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= blackTransitionHalfTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = TransitionStates.Off;
                        blackTransitionImage.color = new Color(0, 0, 0, 0);
                        Player.instance.dead = false;
                    }
                    else
                    {
                        float transition_fraction = 1 - (timeSinceBlackTransitionStarted / blackTransitionHalfTime);
                        blackTransitionImage.color = new Color(0, 0, 0, blackTransitionCurve.Evaluate(transition_fraction));
                    }
                }
            }
        }

        public void StartBlackTransition()
        {
            Player.instance.dead = true;
            timeSinceBlackTransitionStarted = 0f;
            blackTransitionState = TransitionStates.FadeIn;
        }
    }
}
