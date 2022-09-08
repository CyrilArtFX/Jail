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

        ZapTurretProjectile currentProjectile;

		/*[Header("Chainer"), SerializeField]
        ZapTurretChainer chainer;*/

        void Start()
        {
            //  spawn a paused projectile
            FireProjectileTo(null);
            currentProjectile.SetPause(true);

            distToSqr = distance * distance;
        }

        void FixedUpdate()
        {
            //  detect target
            bool wasTargetDetected = hasDetectedTarget;
            hasDetectedTarget = IsDetectingSpirit();

            if (currentProjectile != null)
            {
                //  continue chase if detected again
                if (!wasTargetDetected && hasDetectedTarget)
                {
                    currentProjectile.Chase(Player.instance.Spirit.transform);
                }
                //  return to turret on sight lost
                else if (wasTargetDetected && !hasDetectedTarget)
                {
                    currentProjectile.ReturnToTurret();
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
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.position = projectileSpawnPoint.position;

            //  order chainer to follow projectile
            ZapTurretChainer chainer = projectile.GetComponent<ZapTurretChainer>();
            chainer.TurretAnchor = projectileSpawnPoint;

            //  setup projectile script 
            if (projectile.TryGetComponent(out currentProjectile))
            {
                if (target_transform != null)
                {
                    currentProjectile.Chase(target_transform);
                }
                else
                {
                    currentProjectile.ReturnToTurret();
                }
                currentProjectile.TurretAnchor = projectileSpawnPoint;

                //  link projectile & chainer together
                chainer.Projectile = currentProjectile;
                currentProjectile.Chainer = chainer; 
            }
            else
            {
                Debug.LogError("ZapTurret: Script ZapTurretProjectile wasn't found in the instantiated projectile!");
            }
        }
    }
}
