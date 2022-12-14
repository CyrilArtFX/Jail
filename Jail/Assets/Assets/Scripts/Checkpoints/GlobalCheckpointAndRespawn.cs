using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Jail.SavedObjects;

using Jail.Speedrun;

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

        void Update()
        {
            if (Input.GetButtonDown("RestoreCheckpoint"))
            {
                Respawn();

                if (Player.instance.IsSpirit)
                {
                    Player.instance.FreezeSpirit();
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

            //  speedrun: split zone time
            if (Speedrunner.instance != null)
            {
                Speedrunner.instance.SplitZoneTime();
            }
        }

        public void RestoreCheckpoint()
        {
            foreach (ICheckpointSaver saved_object in savedObjects)
            {
                saved_object.RestoreState();
            }

            if (Player.instance.IsSpirit)
            {
                Player.instance.ReturnToBody(true);
            }

            BlackFade.instance.eventEndOfFadeIn.RemoveListener(RestoreCheckpoint);
        }

        public void Respawn()
        {
            BlackFade.instance.StartFade(FadeType.BothFadesWithRestore);
            BlackFade.instance.eventEndOfFadeIn.AddListener(RestoreCheckpoint);

            //  speedrun: cancel zone time
            if (Speedrunner.instance != null)
            {
                Speedrunner.instance.RevertZoneTime();
                SpeedrunMessager.instance.SetMessage("Checkpoint Used!", false);
            }
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
