using UnityEngine;

using Jail.Utility;

namespace Jail.Interactables.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class BaseTrigger : MonoBehaviour
    {
        public BoxCollider Collider { get; private set; }

        protected Color color = Color.cyan;

        [Header("Base"), SerializeField]
        protected bool isOnce = false;
        [SerializeField]
        protected LayerMask triggerMask;

        int triggerCount = 0;

        protected virtual void Awake()
        {
            Collider = GetComponent<BoxCollider>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (isOnce && triggerCount > 0) return;
            if (!LayerMaskUtils.HasLayer(triggerMask, other.gameObject.layer)) return;

            OnTrigger(other, true);
            triggerCount++;
        }

        void OnTriggerExit(Collider other)
        {
            if (isOnce && triggerCount > 0) return;
            if (!LayerMaskUtils.HasLayer(triggerMask, other.gameObject.layer)) return;

            OnTrigger(other, false);    
            triggerCount++;
        }

        void OnDrawGizmos()
        {
            //  retrieve collider
            if (Collider == null)
            {
                Awake();
            }

            //  draw collider bounds
            Gizmos.color = color;
            Gizmos.DrawWireCube(Collider.bounds.center, Collider.bounds.size);
        }

        protected virtual void OnTrigger(Collider other, bool is_enter) {}
    }
}