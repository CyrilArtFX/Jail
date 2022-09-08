using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail;

namespace Jail.SavedObjects
{
    public class SavedPlayer : SavedObject
    {
        Vector3 savedPosition;
        Quaternion savedRotationModelFlip;

        Player player;

        void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }


        public override void SaveState()
        {
            base.SaveState();

            savedPosition = player.transform.position;
            savedRotationModelFlip = player.modelFlip.localRotation;
        }

        public override void RestoreState()
        {
            base.RestoreState();

            player.body.velocity = Vector3.zero;
            player.transform.position = savedPosition;
            player.modelFlip.localRotation = savedRotationModelFlip;
        }
    }
}
