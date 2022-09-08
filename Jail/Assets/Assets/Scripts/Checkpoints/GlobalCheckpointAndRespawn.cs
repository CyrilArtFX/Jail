using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jail.Puzzler.Inputs;
using Jail.SavedObjects;

namespace Jail
{
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


        List<SavedObject> savedObjects = new List<SavedObject>();

        public static GlobalCheckpointAndRespawn instance;

        float timeSinceBlackTransitionStarted = 0f;
        TransitionStates blackTransitionState = TransitionStates.off;


        void Awake()
        {
            instance = this;
            RetrieveAllSavedObjects();
        }

        void Update()
        {
            if (blackTransitionState != TransitionStates.off)
            {
                if (blackTransitionState == TransitionStates.becomingBlack)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= blackTransitionHalfTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = TransitionStates.stayBlack;
                        blackTransitionImage.color = new Color(0, 0, 0, 1);

                        foreach (SavedObject saved_object in savedObjects)
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
                else if (blackTransitionState == TransitionStates.stayBlack)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= fullBlackTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = TransitionStates.becomingTransparent;
                    }
                }
                else if (blackTransitionState == TransitionStates.becomingTransparent)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= blackTransitionHalfTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = TransitionStates.off;
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
            foreach (SavedObject saved_object in savedObjects)
            {
                saved_object.SaveState();
            }
        }

        public void Respawn()
        {
            Player.instance.dead = true;
            timeSinceBlackTransitionStarted = 0f;
            blackTransitionState = TransitionStates.becomingBlack;
        }


        void RetrieveAllSavedObjects()
        {
            SavedObject[] new_saved_objects = savedObjectsParent.GetComponentsInChildren<SavedObject>(true); 
            foreach(SavedObject saved_object in new_saved_objects)
            {
                savedObjects.Add(saved_object);
            }
        }

        enum TransitionStates
        {
            becomingBlack,
            stayBlack,
            becomingTransparent,
            off
        }
    }
}
