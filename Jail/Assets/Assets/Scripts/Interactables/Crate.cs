using UnityEngine;
using Jail.Utility;
using Jail.SavedObjects;


namespace Jail
{
    public class Crate : MonoBehaviour, ICheckpointSaver
    {
        [SerializeField]
        LayerMask detectionMask = 0;

        [SerializeField]
        PhysicMaterial frictionPM = default, noFrictionPM = default;

        public Rigidbody Body { get; private set; }

        Collider physicCollider;

        bool playerInTrigger = false;

        Vector3 savedPosition;

        void Awake()
        {
            Body = GetComponent<Rigidbody>();
            physicCollider = GetComponent<Collider>();
        }

        void Update()
        {
            if (Mathf.Abs(Body.velocity.y) <= 0.1f && playerInTrigger)
            {
                if (Input.GetButton("Interact"))
                {
                    if (Player.instance.AttachedCrate == null)
                    {
                        GoGrabbedMode();
                    }
                }

                if (Input.GetButtonUp("Interact"))
                {
                    if (Player.instance.AttachedCrate == this)
                    {
                        GoNormalMode();
                    }
                }
            }
            else
            {
                if (Player.instance.AttachedCrate == this)
                {
                    GoNormalMode();
                }
            }
        }

        public void GoGrabbedMode()
        {
            Player.instance.AttachedCrate = this;
            Body.mass = 1.0f;
            physicCollider.material = noFrictionPM;
        }

        public void GoNormalMode()
        {
            Player.instance.AttachedCrate = null;
            Body.mass = 1000000.0f;
            Body.velocity = Vector3.zero;
            physicCollider.material = frictionPM;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;

            playerInTrigger = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectionMask, other.gameObject.layer))
                return;

            playerInTrigger = false;
        }

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
