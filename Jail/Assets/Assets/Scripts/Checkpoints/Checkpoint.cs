using UnityEngine;
using Jail.Utility;

namespace Jail
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        LayerMask detectedLayers = -1;

        bool checkpointPassed = false;

        void OnTriggerEnter(Collider other)
        {
            if (checkpointPassed) return;
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;

            Player.instance.inCheckpoint = this;
        }

        void OnTriggerExit(Collider other)
        {
            if (checkpointPassed) return;
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;

            if (Player.instance.inCheckpoint == this)
            {
                Player.instance.inCheckpoint = null;
            }
        }

        public void UseCheckpoint()
        {
            checkpointPassed = true;
            GlobalCheckpointAndRespawn.instance.SaveCheckpoint();
        }
    }
}
