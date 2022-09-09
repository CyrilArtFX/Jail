using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail.Utility;

namespace Jail
{
    public class DeathZone : MonoBehaviour
    {
        [SerializeField]
        LayerMask detectedLayers = -1;

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;

            GlobalCheckpointAndRespawn.instance.Respawn();
        }
    }
}
