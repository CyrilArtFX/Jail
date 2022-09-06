using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.Interactables
{
    public class ZapTurret : MonoBehaviour
    {
        public bool HasDetectedTarget { get; protected set; }

        [Header("Detection"), SerializeField]
        float distance = 16.0f;

        float distToSqr;

        void Start()
        {
            distToSqr = distance * distance;
        }

        void FixedUpdate()
        {
            //  target only on spirit mode
            if (!Player.instance.IsSpirit) return;

            //  get target transform
            Transform target = Player.instance.SpiritObject.transform;

            //  check for distance
            if ((target.position - transform.position).sqrMagnitude > distToSqr)
            {
                HasDetectedTarget = false;
                return;
            }

            //  detect target
            if (!HasDetectedTarget)
            {
                HasDetectedTarget = true;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = HasDetectedTarget ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, distance);    
        }

        void OnValidate()
        {
            distToSqr = distance * distance;    
        }
    }
}
