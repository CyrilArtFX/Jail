using Jail.LightControl;
using System.Collections;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretProjectile : MonoBehaviour
    {
        public bool IsPulling { get; protected set; }
        public bool IsPaused { get; set; }
        public bool IsChasing { get; set; }
        public ZapTurret Turret { get; set; }
        public Transform WaryPoint => waryPoint;
        public Transform ChainerPoint => chainerPoint;

        public Transform Target { 
            get => target; 
            set {
                if (target == value) return;

                target = value;
                
                //  fade light
                if (target == null)
                {
                    light.FadeOut();
                }
                else
                {
                    light.FadeIn();
                }
            }
        }
        Transform target;

        [Header("Curves"), Tooltip("How the pulling movement should looks like?"), SerializeField]
        AnimationCurve chaseAccelerationCurve;
        [Tooltip("How the pulling movement should looks like?"), SerializeField]
        AnimationCurve pullSpeedCurve;
        [Tooltip("How intense the wave animation should perform over distance on the target?"), SerializeField]
        AnimationCurve waveIntensityCurve;

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
        [SerializeField]
        Transform chainerPoint;
        [SerializeField]
        new LightController light;

        Vector3 pausePos;

        float currentAccelerationTime = 0.0f;
        float t = 0.0f;

        Coroutine returnCoroutine;
        
        void Awake()
        {
            chainer.Projectile = this;
        }

        void Start()
        {
            light.TurnLightOff();
        }

        public void PullToTarget()
        {
            IsChasing = false;
            IsPulling = true;
            returnCoroutine = StartCoroutine(CoroutinePauseForTime(timeBeforeReturn));
        }

        IEnumerator CoroutinePauseForTime(float time)
        {
            pausePos = transform.position;
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
            t = 0.35f;
        }

        void UpdatePullingMovement()
        {
            //  update chainer
            chainer.SplineChainer.Ratio = 1.0f - pullSpeedCurve.Evaluate(t);
            chainer.SplineChainer.DoUpdate();

            //  increase time
            t += Time.fixedDeltaTime * pullSpeed / chainer.SplineChainer.Spline.Length;

            //  get next target point
            Vector3 target_pos = chainer.SplineChainer.Spline.GetPoint(chainer.SplineChainer.Ratio);

            //  look at target
            model.LookAt(target_pos + (transform.position - target_pos).normalized * 2.0f);

            //  move to target
            transform.position = target_pos;

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
            float acceleration_ratio = currentAccelerationTime / accelerationTime;
            float speed = chaseSpeed * chaseAccelerationCurve.Evaluate(acceleration_ratio);
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.fixedDeltaTime * speed);
        }

        void LookAtTarget()
        {
            //  look at target
            Vector3 direction = transform.forward, target_pos = chainer.transform.position;
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
                
                if (Target != null)
                {
                    WavePosition(waveIntensityCurve.Evaluate(1.0f - direction.sqrMagnitude / Turret.DistToSqr));
                }
            }
        }

        void WavePosition(float intensity)
        {
            int unique_id = GetInstanceID();
            transform.position += Mathf.Sin(unique_id + Time.time * 1.27f + Time.time * 0.22f * intensity) * 0.1f * transform.up
                                + Mathf.Cos(unique_id + Time.time * 2.22f) * 0.05f * intensity * transform.forward;
        }

        void FixedUpdate()
        {
            if (IsPaused)
            {
                transform.position = Vector3.Lerp(transform.position, pausePos, transformSmoothSpeed * Time.fixedDeltaTime);
                WavePosition(0.0022f);
                return;
            }

            //  update movement
            if (IsPulling)
            {
                UpdatePullingMovement();
                return;
            }
            
            //  chase
            if (Target != null)
            {
                chainer.Target = Target.position;

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
            Player.instance.GoBackToNormalForm();
            
            //  pull back
            PullToTarget();
        }
    }
}