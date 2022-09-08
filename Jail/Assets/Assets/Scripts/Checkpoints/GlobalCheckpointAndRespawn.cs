using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jail.Puzzler.Inputs;
using Jail.SavedObjects;

namespace Jail
{
    enum TransitionStates
    {
        FadeIn,
        StayBlack,
        FadeOut,
        Off
    }

    public class GlobalCheckpointAndRespawn : MonoBehaviour
    {
        [SerializeField]
        Transform savedObjectsParent = default;

        [Header("Black Transition")]
        [SerializeField]
        Image blackTransitionImage = default;
        [SerializeField]
        float blackTransitionHalfTime = 1f, fullBlackTime = 0.5f;
        [SerializeField]
        AnimationCurve blackTransitionCurve = default;


        List<ICheckpointSaver> savedObjects = new List<ICheckpointSaver>();

        public static GlobalCheckpointAndRespawn instance;

        float timeSinceBlackTransitionStarted = 0f;
        TransitionStates blackTransitionState = TransitionStates.Off;


        void Awake()
        {
            instance = this;
            RetrieveAllSavedObjects();
            SaveCheckpoint();
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

                        foreach (ICheckpointSaver saved_object in savedObjects)
                        {
                            saved_object.RestoreState();
                        }
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


        public void SaveCheckpoint()
        {
            Debug.Log("Checkpoint!");
            foreach (ICheckpointSaver saved_object in savedObjects)
            {
                saved_object.SaveState();
            }
        }

        public void Respawn()
        {
            Player.instance.dead = true;
            timeSinceBlackTransitionStarted = 0f;
            blackTransitionState = TransitionStates.FadeIn;
        }


        void RetrieveAllSavedObjects()
        {
            ICheckpointSaver[] new_saved_objects = savedObjectsParent.GetComponentsInChildren<ICheckpointSaver>(true); 
            foreach(ICheckpointSaver saved_object in new_saved_objects)
            {
                savedObjects.Add(saved_object);
            }
        }
    }
}
