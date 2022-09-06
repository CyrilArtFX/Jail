using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurret : MonoBehaviour
    {
        public bool HasDetectedTarget => hasDetectedTarget;

        [Header("Detection"), SerializeField]
        float distance = 16.0f;
        [SerializeField]
        LayerMask obstacleMask;

        bool hasDetectedTarget = false;
        bool wasRaycastPerformed = false;
        Vector3 raycastStart, raycastHit;
        float distToSqr;

        [Header("Projectile"), SerializeField]
        GameObject projectilePrefab;
        [SerializeField]
        Transform projectileSpawnPoint;

        GameObject currentProjectile;

		/*[Header("Chainer"), SerializeField]
        ZapTurretChainer chainer;*/

        void Start()
        {
            distToSqr = distance * distance;
        }

        void FixedUpdate()
        {
            //  detect target
            hasDetectedTarget = IsDetectingSpirit();

            if (currentProjectile != null)
            {
                //  destroy old projectile if not able to detect target
                if (!hasDetectedTarget)
                {
                    Destroy(currentProjectile);
                }
            }
            else
            {
                //  fire projectile
                if (hasDetectedTarget)
                {
                    FireProjectileTo(Player.instance.Spirit.transform);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = hasDetectedTarget ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, distance);

            if (wasRaycastPerformed)
            {
                Gizmos.DrawLine(raycastStart, raycastHit);
                Gizmos.DrawSphere(raycastHit, 0.25f);
            }
        }

        void OnValidate()
        {
            distToSqr = distance * distance;
        }

        bool IsDetectingSpirit()
        {
            wasRaycastPerformed = false;

            //  target only on spirit mode
            if (!Player.instance.IsSpirit || Player.instance.IsSpiritReturning) 
                return false;

            //  get target transform
            GameObject target = Player.instance.Spirit;

            //  get raycast start & direction
            raycastStart = projectileSpawnPoint.position;
            Vector3 direction = target.transform.position - raycastStart;

            //  check for distance
            if (direction.sqrMagnitude > distToSqr)
                return false;

            //  check for direct eye-contact w/ Spirit
            bool is_hit = Physics.Raycast(raycastStart, direction, out RaycastHit hit_infos, distance, obstacleMask);
            raycastHit = hit_infos.point;
            wasRaycastPerformed = true;
            if (!is_hit || hit_infos.collider.gameObject != target)
                return false;

            return true;
        }

        void FireProjectileTo(Transform target_transform)
        {
            currentProjectile = Instantiate(projectilePrefab);
            currentProjectile.transform.position = projectileSpawnPoint.position;

            //  setup projectile script 
            if (currentProjectile.TryGetComponent(out ZapTurretProjectile script))
            {
                script.targetTransform = target_transform;
                script.turretTransform = transform;
            }

            //  order chainer to follow projectile
            ZapTurretChainer chainer = currentProjectile.GetComponent<ZapTurretChainer>();
            chainer.projectileTarget = gameObject;
        }
    }
}
