using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail.Puzzler.Inputs;

namespace Jail
{
    public class GlobalCheckpointAndRespawn : MonoBehaviour
    {
        [SerializeField] 
        Player player = default;

        [SerializeField]
        Transform puzzleInputsParent = default;


        List<PuzzleBaseInput> puzzleInputs = new List<PuzzleBaseInput>();

        public static GlobalCheckpointAndRespawn instance;


        private void Awake()
        {
            instance = this;
            RetrieveAllPuzzleInputs();
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
            player.Respawn();
            foreach (PuzzleBaseInput puzzle_input in puzzleInputs)
            {
                puzzle_input.ResetState();
            }
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
