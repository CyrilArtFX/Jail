using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail.SavedObjects;


namespace Jail
{
    public class Crate : MonoBehaviour, ICheckpointSaver
    {
        Vector3 savedPosition;

        public void RestoreState()
        {
            transform.position = savedPosition;
        }

        public void SaveState()
        {
            savedPosition = transform.position;
        }
    }
}
