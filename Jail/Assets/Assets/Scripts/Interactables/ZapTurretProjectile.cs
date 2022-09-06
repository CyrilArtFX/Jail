using System.Collections;
using UnityEngine;

namespace Jail.Interactables
{
    public class ZapTurretProjectile : MonoBehaviour
    {
        public Transform TurretTransform { get; set; }
        public Transform TargetTransform { get; set; }

        [SerializeField]
        float moveSpeed = 50.0f;
        
        void FixedUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetTransform.position, Time.fixedDeltaTime * moveSpeed);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != Player.instance.Spirit) return;

            Player.instance.GoBackToNormalForm();
        }
    }
}