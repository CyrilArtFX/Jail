using System.Collections;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretProjectile : MonoBehaviour
    {
        public ZapTurretChainer Chainer { get; set; }
        public Transform Target { get; set; }
        public bool IsPulling { get; protected set; }
        public bool IsPaused { get; set; }

        Vector3 target;

        [Tooltip("How the pulling movement should looks like?"), SerializeField]
        AnimationCurve chaseAccelerationCurve;
        [Tooltip("How the pulling movement should looks like?"), SerializeField]
        AnimationCurve pullSpeedCurve;

        [Tooltip("How fast the chase movement should be?"), SerializeField]
        float chaseSpeed = 10.0f;
        [Tooltip("How much time should it takes to be at full speed?"), SerializeField]
        float accelerationTime = 1.0f;
        [Tooltip("How fast the return movement should be?"), SerializeField]
        float pullSpeed = 15.0f;
        [Tooltip("How much time should it wait before returning to its default position after chase?"), SerializeField]
        float timeBeforeReturn = 1.0f;
        
        float currentAccelerationTime = 0.0f;
        float t = 0.0f;

        Coroutine returnCoroutine;
        
        public void PullToTarget()
        {
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
            IsPaused = false;

            //  reset acceleration
            currentAccelerationTime = 0.0f;

            //  reset pulling variables
            Chainer.SplineChainer.Ratio = 1.0f;
            t = 0.0f;
        }

        void UpdatePullingMovement()
        {
            //  update chainer
            Chainer.SplineChainer.Ratio = 1.0f - pullSpeedCurve.Evaluate(t);
            Chainer.SplineChainer.DoUpdate();

            //  increase time
            t += Time.fixedDeltaTime * pullSpeed / Chainer.SplineChainer.Spline.Length;

            //  auto-pause
            if (Chainer.SplineChainer.Ratio == 0.0f)
            {
                IsPaused = true;
            }
            
            //  move towards target
            target = Chainer.SplineChainer.Spline.GetPoint(Chainer.SplineChainer.Ratio);
            transform.position = target;
        }

        void UpdateChase()
        {
            //  acceleration
            currentAccelerationTime = Mathf.Min(accelerationTime, currentAccelerationTime + Time.fixedDeltaTime);
            
            //  move towards target
            float speed = chaseSpeed * chaseAccelerationCurve.Evaluate(currentAccelerationTime / accelerationTime);
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.fixedDeltaTime * speed);
        }

        void FixedUpdate()
        {
            if (IsPaused) return;

            //  update movement
            if (IsPulling)
            {
                UpdatePullingMovement();
            }
            else if (Target != null)
            {
                UpdateChase();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != Player.instance.Spirit) return;

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