using UnityEngine;
using Jail.Utility;
using Jail.SavedObjects;


namespace Jail
{
    public class Crate : MonoBehaviour, ICheckpointSaver
    {
        [SerializeField]
        LayerMask detectionMask;

        Rigidbody body;

        bool playerInTrigger = false;

        Vector3 savedPosition;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (body.velocity.y == 0.0f && playerInTrigger)
            {
                if (Input.GetButton("Interact"))
                {
                    if (!Player.instance.attachedCrates.Contains(body))
                    {
                        Player.instance.attachedCrates.Add(body);
                        body.mass = 0.01f;
                    }
                }

                if (Input.GetButtonUp("Interact"))
                {
                    if (Player.instance.attachedCrates.Contains(body))
                    {
                        Player.instance.attachedCrates.Remove(body);
                        body.velocity = Vector3.zero;
                        body.mass = 1000000.0f;
                    }
                }
            }
            else
            {
                if (Player.instance.attachedCrates.Contains(body))
                {
                    Player.instance.attachedCrates.Remove(body);
                    body.velocity = Vector3.zero;
                    body.mass = 1000000.0f;
                }
            }
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
