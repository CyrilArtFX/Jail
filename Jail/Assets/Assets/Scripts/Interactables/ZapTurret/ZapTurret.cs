using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurret : MonoBehaviour
    {
        public float DistToSqr => distToSqr;
        public bool HasDetectedTarget => hasDetectedTarget;

        [Header("Detection"), SerializeField]
        float distance = 16.0f;
        [SerializeField]
        LayerMask playerObstacleMask, spiritObstacleMask;

        bool hasDetectedTarget = false;
        bool wasRaycastPerformed = false;
        Vector3 raycastStart, raycastHit;
        float distToSqr;

        [Header("Projectile")]
        [SerializeField]
        Transform projectileSpawnPoint;
        [SerializeField]
        Animator animator;

        [SerializeField]
        ZapTurretProjectile currentProjectile;

        void Start()
        {
            currentProjectile.Turret = this;

            //  compute square of distance
            distToSqr = distance * distance;
        }

        void FixedUpdate()
        {
            //  detect target
            bool wasTargetDetected = hasDetectedTarget;

            //  detect targets
            bool can_act = true;
            if (Player.instance.IsSpirit && !Player.instance.IsSpiritReturning && IsDetectingTarget(Player.instance.Spirit, spiritObstacleMask))
            {
                //  priority on targeting spirit
                hasDetectedTarget = true;
            }
            else if (IsDetectingTarget(Player.instance.gameObject, playerObstacleMask, Player.instance.HeadPoint))
            {
                //  target the body otherwise
                hasDetectedTarget = false;

                //  turn back if was chasing the spirit
                if (currentProjectile.IsChasing)
                {
                    currentProjectile.PullToTarget();
                }

                //  don't target the body
                can_act = false;
            }
            else
            {
                hasDetectedTarget = false;
                currentProjectile.Target = null;
            }

            //  get anim target speed
            float anim_target_speed = .33f;
            if (currentProjectile.Target == Player.instance.Spirit.transform)
            {
                anim_target_speed = 2.22f;
            }
            else if (currentProjectile.Target != null)
            {
                anim_target_speed = 1.0f;
            }

            //  animation speed
            float anim_speed = Mathf.Lerp(animator.GetFloat("IdleSpeed"), anim_target_speed, Time.fixedDeltaTime * 10.0f);
            animator.SetFloat("IdleSpeed", anim_speed);

            if (can_act)
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

        bool IsDetectingTarget(GameObject target, LayerMask mask, Transform target_point = null)
        {
            wasRaycastPerformed = false;

            //  retrieve default transform
            if (target_point == null)
            {
                target_point = target.transform;
            }

            //  get raycast start & direction
            raycastStart = currentProjectile.WaryPoint.position;
            Vector3 direction = target_point.position - raycastStart;

            //  check for distance
            if (direction.sqrMagnitude > distToSqr)
                return false;

            //  check for direct eye-contact w/ Spirit
            bool is_hit = Physics.Raycast(raycastStart, direction, out RaycastHit hit_infos, distance, mask, QueryTriggerInteraction.Ignore);
            raycastHit = hit_infos.point;
            wasRaycastPerformed = true;
            if (!is_hit || hit_infos.collider.gameObject != target)
                return false;

            currentProjectile.Target = target_point;
            return true;
        }
    }
}
