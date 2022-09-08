using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail.Utility;

namespace Jail
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        LayerMask detectedLayers = -1;

        bool checkpointPassed = false;

        

        void OnTriggerStay(Collider other)
        {
            if (checkpointPassed) return;
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;
            Player.instance.inCheckpoint = this;
        }

        public void UseCheckpoint()
        {
            checkpointPassed = true;
            GlobalCheckpointAndRespawn.instance.SaveCheckpoint();
        }
    }
}
