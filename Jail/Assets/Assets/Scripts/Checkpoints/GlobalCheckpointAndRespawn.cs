using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Jail.SavedObjects;

namespace Jail
{
    public class GlobalCheckpointAndRespawn : MonoBehaviour
    {
        [SerializeField]
        Transform savedObjectsParent = default;

        List<ICheckpointSaver> savedObjects = new List<ICheckpointSaver>();

        public static GlobalCheckpointAndRespawn instance;


        void Awake()
        {
            instance = this;
            RetrieveAllSavedObjects();
            SaveCheckpoint();
        }


        public void SaveCheckpoint()
        {
            Debug.Log("Checkpoint!");
            foreach (ICheckpointSaver saved_object in savedObjects)
            {
                saved_object.SaveState();
            }
        }

        public void RestoreCheckpoint()
        {
            foreach (ICheckpointSaver saved_object in savedObjects)
            {
                saved_object.RestoreState();
            }

            BlackFade.instance.eventEndOfFadeIn.RemoveListener(RestoreCheckpoint);
        }

        public void Respawn()
        {
            BlackFade.instance.StartFade(FadeType.BothFadesWithRestore);
            BlackFade.instance.eventEndOfFadeIn.AddListener(RestoreCheckpoint);
        }


        void RetrieveAllSavedObjects()
        {
            ICheckpointSaver[] new_saved_objects = savedObjectsParent.GetComponentsInChildren<ICheckpointSaver>(true);
            foreach (ICheckpointSaver saved_object in new_saved_objects)
            {
                savedObjects.Add(saved_object);
            }
        }
    }
}
