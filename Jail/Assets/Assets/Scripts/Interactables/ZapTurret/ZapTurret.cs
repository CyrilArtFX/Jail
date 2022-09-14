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

        void Start()
        {
            //  spawn a paused projectile
            SpawnProjectile();

            //  compute square of distance
            distToSqr = distance * distance;
        }

        void FixedUpdate()
        {
            //  detect target
            bool wasTargetDetected = hasDetectedTarget;
            hasDetectedTarget = IsDetectingSpirit();

            if (currentProjectile != null)
            {
                //  chase spirit if not pulling
                if (!wasTargetDetected && hasDetectedTarget)
                {
                    if (!currentProjectile.IsPulling)
                    {
                        currentProjectile.Chase(Player.instance.Spirit.transform);
                    }
                    else
                    {
                        //  ensure it can chase next frame if possible
                        hasDetectedTarget = false;
                    }
                }
                //  return to turret on sight lost
                else if (wasTargetDetected && !hasDetectedTarget)
                {
                    currentProjectile.PullToTarget();
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

        void SpawnProjectile()
        {
            //  spawn projectile
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.position = projectileSpawnPoint.position;

            //  get chainer
            ZapTurretChainer chainer = projectileSpawnPoint.GetComponent<ZapTurretChainer>();

            //  setup projectile script 
            currentProjectile = projectile.GetComponent<ZapTurretProjectile>();
            currentProjectile.IsPaused = true;

            //  link projectile & chainer together
            chainer.Projectile = currentProjectile;
            currentProjectile.Chainer = chainer; 
        }
    }
}
