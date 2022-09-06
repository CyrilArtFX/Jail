using UnityEngine;
using Jail.Utility;

namespace Jail
{
    public class PlayerTrigger : MonoBehaviour
    {
        [SerializeField]
        LayerMask detectedLayers = -1;

        bool detect = false;
        public bool ObstacleDetected => detect;

        public static PlayerTrigger instance;

        void Awake()
        {
            instance = this;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;
            detect = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;
            detect = false;
        }
    }
}
