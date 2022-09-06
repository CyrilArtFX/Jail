using System.Collections;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretProjectile : MonoBehaviour
    {
        public Transform turretTransform;
        public Transform targetTransform;

        [SerializeField]
        float moveSpeed = 50.0f;
        
        void FixedUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, Time.fixedDeltaTime * moveSpeed);
            transform.LookAt(targetTransform);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != Player.instance.Spirit) return;

            Player.instance.GoBackToNormalForm();
            Destroy(gameObject);
        }
    }
}