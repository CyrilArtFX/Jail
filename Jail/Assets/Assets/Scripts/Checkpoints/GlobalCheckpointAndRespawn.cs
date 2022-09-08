using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jail.Puzzler.Inputs;

namespace Jail
{
    public class GlobalCheckpointAndRespawn : MonoBehaviour
    {
        [SerializeField] 
        Player player = default;

        [SerializeField]
        Transform puzzleInputsParent = default;

        [Header("Black Transition")]
        [SerializeField]
        Image blackTransitionImage = default;
        [SerializeField]
        float blackTransitionHalfTime = 1f, fullBlackTime = 0.5f;
        [SerializeField]
        AnimationCurve blackTransitionCurve = default;


        List<PuzzleBaseInput> puzzleInputs = new List<PuzzleBaseInput>();

        public static GlobalCheckpointAndRespawn instance;

        float timeSinceBlackTransitionStarted = 0f;
        int blackTransitionState = 0;


        void Awake()
        {
            instance = this;
            RetrieveAllPuzzleInputs();
        }

        void Update()
        {
            if (blackTransitionState > 0)
            {
                if (blackTransitionState == 1)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= blackTransitionHalfTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = 2;
                        blackTransitionImage.color = new Color(0, 0, 0, 1);

                        player.Respawn();
                        foreach (PuzzleBaseInput puzzle_input in puzzleInputs)
                        {
                            puzzle_input.ResetState();
                        }
                    }
                    else
                    {
                        float transition_fraction = timeSinceBlackTransitionStarted / blackTransitionHalfTime;
                        blackTransitionImage.color = new Color(0, 0, 0, blackTransitionCurve.Evaluate(transition_fraction));
                    }
                }
                else if (blackTransitionState == 2)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if(timeSinceBlackTransitionStarted >= fullBlackTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = 3;
                    }
                }
                else if (blackTransitionState == 3)
                {
                    timeSinceBlackTransitionStarted += Time.deltaTime;
                    if (timeSinceBlackTransitionStarted >= blackTransitionHalfTime)
                    {
                        timeSinceBlackTransitionStarted = 0f;
                        blackTransitionState = 0;
                        blackTransitionImage.color = new Color(0, 0, 0, 0);
                        player.dead = false;
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
            Debug.Log("Checkpoint !");
            player.SavePosition();
            foreach(PuzzleBaseInput puzzle_input in puzzleInputs)
            {
                puzzle_input.SaveState();
            }
        }

        public void Respawn()
        {
            player.dead = true;
            timeSinceBlackTransitionStarted = 0f;
            blackTransitionState = 1;
        }


        void RetrieveAllPuzzleInputs()
        {
            for(int i = 0; i < puzzleInputsParent.childCount; i++)
            {
                if(puzzleInputsParent.GetChild(i).TryGetComponent(out PuzzleBaseInput puzzle_input))
                {
                    puzzleInputs.Add(puzzle_input);
                }
            }
        }
    }
}
