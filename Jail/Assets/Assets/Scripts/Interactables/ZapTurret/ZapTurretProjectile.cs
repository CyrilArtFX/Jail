using System.Collections;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretProjectile : MonoBehaviour
    {
        public Transform Target { get; set; }
        public bool IsPulling { get; protected set; }
        public bool IsPaused { get; set; }
        public bool IsChasing { get; set; }
        public Transform WaryPoint => waryPoint;

        Vector3 target;

        [Header("Curves"), Tooltip("How the pulling movement should looks like?"), SerializeField]
        AnimationCurve chaseAccelerationCurve;
        [Tooltip("How the pulling movement should looks like?"), SerializeField]
        AnimationCurve pullSpeedCurve;

        [Header("Stats"), Tooltip("How fast the chase movement should be?"), SerializeField]
        float chaseSpeed = 10.0f;
        [Tooltip("How much time should it takes to be at full speed?"), SerializeField]
        float accelerationTime = 1.0f;
        [Tooltip("How fast the return movement should be?"), SerializeField]
        float pullSpeed = 15.0f;
        [Tooltip("How much time should it wait before returning to its default position after chase?"), SerializeField]
        float timeBeforeReturn = 1.0f;

        [SerializeField]
        float transformSmoothSpeed = 4.0f;

        [Header("References"), SerializeField]
        Transform model;
		[SerializeField]
        ZapTurretChainer chainer;
		[SerializeField]
        Transform waryPoint;
        
        float currentAccelerationTime = 0.0f;
        float t = 0.0f;

        Coroutine returnCoroutine;
        
        void Awake()
		{
            chainer.Projectile = this;
		}

        public void PullToTarget()
        {
            IsChasing = false;
            IsPulling = true;
            returnCoroutine = StartCoroutine(CoroutinePauseForTime(timeBeforeReturn));
        }

        IEnumerator CoroutinePauseForTime(float time)
        {
            IsPaused = true;

            yield return new WaitForSeconds(time);

            IsPaused = false;
        }

        public void Chase(Transform target)
        {
            //  stop previous coroutine
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
            }

            //  setup variables
            Target = target;
            IsPulling = false;
            IsChasing = true;
            IsPaused = false;

            //  reset acceleration
            currentAccelerationTime = 0.0f;

            //  reset pulling variables
            t = 0.0f;
        }

        void UpdatePullingMovement()
        {
            //  update chainer
            chainer.SplineChainer.Ratio = 1.0f - pullSpeedCurve.Evaluate(t);
            chainer.SplineChainer.DoUpdate();

            //  increase time
            t += Time.fixedDeltaTime * pullSpeed / chainer.SplineChainer.Spline.Length;

            //  get next target point
            target = chainer.SplineChainer.Spline.GetPoint(chainer.SplineChainer.Ratio);

            //  look at target
            model.LookAt(target + (transform.position - target).normalized * 2.0f);

            //  move to target
            transform.position = target;

			//  auto-pause
			if (chainer.SplineChainer.Ratio == 0.0f || (transform.position - waryPoint.position).magnitude <= 0.5f)
			{
                IsPulling = false;
                chainer.SplineChainer.Ratio = 1.0f;
			}
        }

        void UpdateChase()
        {
            //  acceleration
            currentAccelerationTime = Mathf.Min(accelerationTime, currentAccelerationTime + Time.fixedDeltaTime);
            
            //  move towards target
            float speed = chaseSpeed * chaseAccelerationCurve.Evaluate(currentAccelerationTime / accelerationTime);
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.fixedDeltaTime * speed);
        }

        void LookAtTarget()
        {
            //  look at target
            Vector3 direction = Vector3.down, target_pos = chainer.transform.position;
            if (Target != null)
            {
                direction = Target.position - model.position;
                target_pos = waryPoint.position;
            }
            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(direction), transformSmoothSpeed * Time.fixedDeltaTime);
            
            //  move to target
            if (!IsChasing)
			{
                transform.position = Vector3.Lerp(transform.position, target_pos, transformSmoothSpeed * Time.fixedDeltaTime);
			}
        }

        void FixedUpdate()
        {
            if (IsPaused) return;

            //  update movement
            if (IsPulling)
            {
                UpdatePullingMovement();
                return;
            }
            
            //  chase
            if (Target != null)
            {
                if (IsChasing)
                {
                    UpdateChase();
                }
            }

            LookAtTarget();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != Player.instance.Spirit) return;
            if (Player.instance.IsSpiritReturning) return;

            //  kill spirit
            Player.instance.GoBackToNormalForm(true);
            
            //  pull back
            PullToTarget();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target);
            Gizmos.DrawWireSphere(target, .5f);
        }
    }
}