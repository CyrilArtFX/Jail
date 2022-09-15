using Jail.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail;
using Jail.SavedObjects;

namespace Jail
{
    public class Player : MonoBehaviour, ICheckpointSaver
    {
        enum CrateAction
        {
            Pushing,
            Pulling,
            None
        }

        public GameObject Spirit => spiritObject;
        public bool IsSpiritReturning => spiritReturning || spiritDissolving;

        [Header("Solid Body")]
        [SerializeField]
        Transform modelFlip = default;

        [Header("Spirit")]
        [SerializeField]
        GameObject spiritObject = default;
        [SerializeField]
        Transform spiritModelFlip = default;
        [SerializeField]
        DissolveObject spiritDissolve = default;
        [SerializeField]
        ParticleSystem spiritParticles = default;

        [Header("Parameters")]
        [SerializeField, Range(0.0f, 100.0f)]
        float maxSpeed = 13.0f, maxClimbSpeed = 2.0f, maxSpiritSpeed = 8.0f, maxCrateSpeed = 5.0f;
        [SerializeField, Range(0.0f, 100.0f)]
        float maxAcceleration = 10.0f, maxAirAcceleration = 1.0f, maxClimbAcceleration = 20.0f, maxSpiritAcceleration = 15.0f;
        [SerializeField, Range(0.0f, 10.0f)]
        float jumpHeight = 2.0f;
        [SerializeField, Range(0.0f, 90.0f)]
        float maxGroundAngle = 25.0f, maxStairsAngle = 50.0f;
        [SerializeField, Range(0.0f, 100.0f)]
        float maxSnapSpeed = 100.0f;
        [SerializeField, Min(0.0f)]
        float probeDistance = 1.0f;
        [SerializeField, Range(90.0f, 170.0f)]
        float maxClimbAngle = 140.0f;
        [SerializeField]
        LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, ladderMask = 0, realGroundMask = 0;
        [SerializeField, Min(0.0f)]
        float modelAlignSpeed = 180.0f, modelFlipSpeed = 1080.0f;

        [Header("Others")]
        [SerializeField]
        bool maintainButtonForClimb = false;
        [SerializeField]
        Transform camFocus = default;

        Rigidbody body, connectedBody, previousConnectedBody;
        Animator animator;

        Vector2 playerInput;
        bool desiredJump, desiresClimbing, requestClimbing, desireSpirit, desireNormal;

        Vector3 velocity, connectionVelocity;
        Vector3 groundNormal, contactNormal, steepNormal, climbNormal, lastClimbNormal;
        Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

        int groundContactCount, steepContactCount, climbContactCount, realGroundContactCount;
        Ladder currentLadder;

        public Crate AttachedCrate { get; set; }
        CrateAction crateAction = CrateAction.None;

        bool OnGround => groundContactCount > 0;
        bool OnSteep => steepContactCount > 0;
        bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

        bool OnRealGround => realGroundContactCount > 0 && !Climbing;

        public bool IsSpirit => spirit;

        bool spirit = false, spiritReturning = false, spiritDissolving = false, spiritDisabled = false;
        float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
        int stepsSinceLastGrounded, stepsSinceLastJump, stepsSinceLastClimbRequest;
        Vector3 connectionWorldPosition, connectionLocalPosition;

        [SerializeField]
        float gravityModifier = 2.0f;

        public static Player instance;

        CapsuleCollider spiritCollider = default;
        Rigidbody spiritBody = default;

        [HideInInspector]
        public Checkpoint inCheckpoint = null;
        [HideInInspector]
        public bool disableCommands = false;

        Vector3 savedPosition;
        Quaternion savedRotationModelFlip;


        [Header("Spirit Returning Parameters")]
        [SerializeField, Tooltip("The average speed of the spirit while returning, in meter per seconds")]
        float spiritReturningAverageSpeed = 5.0f;
        [SerializeField, Tooltip("The distance between spirit and body by the time")]
        AnimationCurve spiritReturningCurve = default;
        [SerializeField, Tooltip("The time after spirit returning when spirit is disabled")]
        float disableSpiritTime = 0.5f;

        Vector3 spiritPosAtStartReturning;
        float timeForSpiritToReturn;
        float timeSinceSpiritReturningStart;

        void OnValidate()
        {
            minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
            minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
        }

        void Awake()
        {
            instance = this;

            body = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            body.useGravity = false;

            spiritBody = spiritObject.GetComponent<Rigidbody>();
            spiritCollider = spiritObject.GetComponent<CapsuleCollider>();
            spiritBody.useGravity = false;
            spiritObject.SetActive(false);
            OnValidate();
        }

        void Update()
        {
            camFocus.localPosition = spirit ? spiritObject.transform.localPosition : Vector3.zero;

            if (disableCommands) return;

            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Climbing || spirit ? Input.GetAxis("UpDown") : 0.0f;
            playerInput = Vector3.ClampMagnitude(playerInput, 1.0f);

            if (maintainButtonForClimb)
            {
                desiresClimbing = Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f;
            }
            else
            {
                desiresClimbing |= Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f;
                requestClimbing = Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f;
            }

            desiredJump |= Input.GetButtonDown("Jump");

            desireSpirit |= Input.GetButtonDown("Spirit");

            if (spirit)
            {
                desiresClimbing = false;
                requestClimbing = false;
                desiredJump = false;

                desireNormal |= Input.GetButtonDown("Spirit");
            }

            UpdateRotations();


            animator.SetFloat("Speed", Mathf.Abs(body.velocity.z));
            animator.SetBool("Pushing", crateAction == CrateAction.Pushing);
            animator.SetBool("Pulling", crateAction == CrateAction.Pulling);
            animator.SetBool("Falling", !Climbing && !OnGround && body.velocity.y < -0.01f);
            animator.SetBool("Climbing", Climbing);


            if (spiritReturning)
            {
                timeSinceSpiritReturningStart += Time.deltaTime;

                if (timeSinceSpiritReturningStart >= timeForSpiritToReturn)
                {
                    spiritCollider.isTrigger = false;
                    spiritReturning = false;

                    spiritDissolve.ForceNoDissolve();

                    spiritObject.SetActive(false);
                    spirit = false;

                    if (disableSpiritTime > 0.0f)
                    {
                        StartCoroutine(DisableSpirit());
                    }
                }
                else
                {
                    float time_scaled = timeSinceSpiritReturningStart * (1 / timeForSpiritToReturn);
                    float return_fraction = spiritReturningCurve.Evaluate(time_scaled);

                    spiritObject.transform.localPosition = spiritPosAtStartReturning * (1 - return_fraction);
                }
            }
        }

        void UpdateRotations()
        {
            //  make the spirit head towards where he goes
            if (spirit)
            {
                Vector3 spirit_rotation_plane_normal;
                if (spiritReturning)
                {
                    spirit_rotation_plane_normal = -spiritObject.transform.localPosition.normalized;
                }
                else
                {
                    if (spiritBody.velocity == Vector3.zero || playerInput == Vector2.zero)
                    {
                        spirit_rotation_plane_normal = spiritObject.transform.up;
                    }
                    else
                    {
                        spirit_rotation_plane_normal = spiritBody.velocity.normalized;
                    }
                }

                Quaternion spirit_rotation = spiritObject.transform.localRotation;
                if (modelAlignSpeed > 0.0f)
                {
                    spirit_rotation = AlignModelRotation(spirit_rotation_plane_normal, spirit_rotation);
                }
                spiritObject.transform.localRotation = spirit_rotation;
            }


            //  make the spirit or the player face toward where he goes
            Quaternion flip_rotation = spirit ? spiritModelFlip.localRotation : modelFlip.localRotation;
            if (playerInput.x != 0 && !Climbing && crateAction != CrateAction.Pulling)
            {
                float second_rotation_y = flip_rotation.eulerAngles.y;
                second_rotation_y = Mathf.Clamp(second_rotation_y - playerInput.x * modelFlipSpeed * Time.deltaTime, 0.0f, 180.0f);
                flip_rotation = Quaternion.Euler(0.0f, second_rotation_y, 0.0f);
            }
            else
            {
                if (Climbing)
                {
                    if (currentLadder != null)
                    {
                        flip_rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    }
                }
            }
            if (Time.timeScale != 0.0f)
            {
                if (spirit)
                {
                    spiritModelFlip.localRotation = flip_rotation;
                }
                else
                {
                    modelFlip.localRotation = flip_rotation;
                }
            }
        }

        Quaternion AlignModelRotation(Vector3 rotation_axis, Quaternion rotation)
        {
            Vector3 model_axis = spiritObject.transform.up;
            float dot = Mathf.Clamp(Vector3.Dot(model_axis, rotation_axis), -1.0f, 1.0f);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            float max_angle = modelAlignSpeed * Time.deltaTime;

            Quaternion new_alignment = Quaternion.FromToRotation(model_axis, rotation_axis) * rotation;
            if (angle <= max_angle || Climbing)
                return new_alignment;
            else
                return Quaternion.SlerpUnclamped(rotation, new_alignment, max_angle / angle);
        }

        void FixedUpdate()
        {
            if (disableCommands)
            {
                body.velocity = Vector3.zero;
                return;
            }

            UpdateState();

            AdjustVelocity();

            if (desireSpirit)
            {
                desireSpirit = false;
                if (!desiredJump && !Climbing && OnGround && !spirit && !spiritDisabled)
                {
                    TransformToSpirit();
                }
            }

            if (desiredJump)
            {
                desiredJump = false;
                Jump();
            }

            Vector3 body_velocity = velocity;

            if (spirit)
            {
                spiritBody.velocity = velocity;
                body_velocity = Vector3.zero;

                if (desireNormal)
                {
                    desireNormal = false;
                    GoBackToNormalForm(false);
                }
            }


            if (Climbing)
            {
                body_velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
            }
            else if (OnGround && velocity.sqrMagnitude < 0.01f)
            {
                body_velocity += contactNormal * (Vector3.Dot(Physics.gravity, contactNormal) * Time.deltaTime);
            }
            else if (desiresClimbing && OnGround)
            {
                body_velocity += (Physics.gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
            }
            else if (!spirit)
            {
                body_velocity += Physics.gravity * gravityModifier * Time.deltaTime;
            }

            body.velocity = body_velocity;

            if (AttachedCrate != null)
            {
                if (!OnRealGround)
                {
                    AttachedCrate.GoNormalMode();
                }
                else
                {
                    Vector3 crate_velocity = new Vector3(0.0f, 0.0f, body_velocity.z);
                    AttachedCrate.Body.velocity = crate_velocity;
                }
            }

            ClearState();
        }

        void ClearState()
        {
            lastContactNormal = contactNormal;
            lastSteepNormal = steepNormal;
            lastConnectionVelocity = connectionVelocity;

            groundContactCount = steepContactCount = climbContactCount = realGroundContactCount = 0;
            groundNormal = contactNormal = steepNormal = connectionVelocity = climbNormal = Vector3.zero;

            previousConnectedBody = connectedBody;
            connectedBody = null;
        }

        void UpdateState()
        {
            stepsSinceLastGrounded++;
            stepsSinceLastJump++;
            stepsSinceLastClimbRequest = requestClimbing ? 0 : stepsSinceLastClimbRequest + 1;

            velocity = spirit ? spiritBody.velocity : body.velocity;

            if (CheckClimbing() || OnGround || SnapToGround() || CheckSteepContact())
            {
                stepsSinceLastGrounded = 0;
                if (groundContactCount > 1)
                {
                    contactNormal.Normalize();
                }
            }
            else
            {
                contactNormal = Vector3.up;
            }

            if (connectedBody)
            {
                if (connectedBody.isKinematic || connectedBody.mass >= body.mass)
                {
                    UpdateConnectionState();
                }
            }

            //  use the checkpoint if the player is on the ground and not climbing
            if (inCheckpoint)
            {
                if (!Climbing && OnGround)
                {
                    inCheckpoint.UseCheckpoint();
                    inCheckpoint = null;
                }
            }

            //  set the crateAction value
            if (AttachedCrate != null)
            {
                if (Mathf.Abs(body.velocity.z) < 0.01f)
                {
                    crateAction = CrateAction.None;
                }
                else
                {
                    if (AttachedCrate.transform.position.z < transform.position.z)
                    {
                        if (body.velocity.z > 0.0f)
                        {
                            crateAction = CrateAction.Pulling;
                        }
                        else
                        {
                            crateAction = CrateAction.Pushing;
                        }
                    }
                    else
                    {
                        if (body.velocity.z > 0.0f)
                        {
                            crateAction = CrateAction.Pushing;
                        }
                        else
                        {
                            crateAction = CrateAction.Pulling;
                        }
                    }
                }
            }
            else
            {
                crateAction = CrateAction.None;
            }
        }

        void AdjustVelocity()
        {
            float acceleration, speed;
            Vector3 x_axis;
            if (spirit)
            {
                acceleration = maxSpiritAcceleration;
                speed = maxSpiritSpeed;
                x_axis = Vector3.back;
            }
            else if (Climbing)
            {
                acceleration = maxClimbAcceleration;
                speed = maxClimbSpeed;
                x_axis = currentLadder != null ? Vector3.back : Vector3.zero;
            }
            else
            {
                acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
                speed = (AttachedCrate != null && OnRealGround) ? maxCrateSpeed : maxSpeed;
                x_axis = Vector3.back;
            }
            x_axis = ProjectDirectionOnPlane(x_axis, contactNormal);

            Vector3 relative_velocity = velocity - connectionVelocity;

            Vector2 adjustment;
            adjustment.x = playerInput.x * speed - Vector3.Dot(relative_velocity, x_axis);
            adjustment.y = Climbing || spirit ? playerInput.y * speed - Vector3.Dot(relative_velocity, Vector3.up) : 0.0f;
            adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

            velocity += x_axis * adjustment.x;

            if (Climbing || spirit)
            {
                velocity += Vector3.up * adjustment.y;
            }
            if (IsSpiritReturning)
            {
                velocity = Vector3.zero;
            }
        }

        bool SnapToGround()
        {
            if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
            {
                return false;
            }
            float speed = velocity.magnitude;
            if (speed > maxSnapSpeed)
            {
                return false;
            }
            if (!Physics.Raycast(body.position, -Vector3.up, out RaycastHit hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }
            float up_dot = Vector3.Dot(Vector3.up, hit.normal);
            if (up_dot < GetMinDot(hit.collider.gameObject.layer))
            {
                return false;
            }

            groundContactCount = 1;
            contactNormal = hit.normal;
            float dot = Vector3.Dot(velocity, hit.normal);
            if (dot > 0.0f)
            {
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            connectedBody = hit.rigidbody;
            return true;
        }

        void Jump()
        {
            Vector3 jump_direction;
            if (OnGround)
            {
                jump_direction = Vector3.up;
                if (Climbing && currentLadder != null)
                {
                    jump_direction = Vector3.zero;
                    currentLadder.DesactiveClimbable();
                    currentLadder = null;
                }
            }
            else return;

            stepsSinceLastJump = 0;
            float jumpSpeed = Mathf.Sqrt(2.0f * Physics.gravity.magnitude * jumpHeight * gravityModifier * (1.0f + jumpHeight / 25.0f));
            jump_direction = (jump_direction + Vector3.up).normalized;

            float alignedSpeed = Vector3.Dot(velocity, jump_direction);
            if (alignedSpeed > 0.0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0.0f);
            }

            velocity += jump_direction * jumpSpeed;

            animator.SetTrigger("Jumping");
        }

        bool CheckSteepContact()
        {
            if (steepContactCount > 1)
            {
                steepNormal.Normalize();
                float up_dot = Vector3.Dot(Vector3.up, steepNormal);
                if (up_dot >= minGroundDotProduct)
                {
                    groundContactCount = 1;
                    contactNormal = steepNormal;
                    return true;
                }
            }
            return false;
        }

        bool CheckClimbing()
        {
            if (Climbing)
            {
                if (stepsSinceLastClimbRequest > 10 && groundNormal != Vector3.zero) desiresClimbing = false;
                if (climbContactCount > 1)
                {
                    climbNormal.Normalize();
                    float up_dot = Vector3.Dot(Vector3.up, climbNormal);
                    if (up_dot >= minGroundDotProduct)
                    {
                        climbNormal = lastClimbNormal;
                    }
                }
                groundContactCount = 1;
                contactNormal = climbNormal;
                return true;
            }
            else
            {
                if (!maintainButtonForClimb && !requestClimbing && stepsSinceLastClimbRequest > 2 && climbNormal == Vector3.zero)
                {
                    desiresClimbing = false;
                }
            }
            return false;
        }

        void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void EvaluateCollision(Collision collision)
        {
            int layer = collision.gameObject.layer;
            float min_dot = GetMinDot(layer);
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(Vector3.up, normal);
                if (upDot >= min_dot)
                {
                    if (LayerMaskUtils.HasLayer(realGroundMask, collision.gameObject.layer))
                    {
                        realGroundContactCount += 1;
                    }

                    groundContactCount += 1;
                    contactNormal += normal;
                    connectedBody = collision.rigidbody;
                    groundNormal += normal;
                }
                else
                {
                    if (upDot > -0.01f)
                    {
                        steepContactCount += 1;
                        steepNormal += normal;
                        if (groundContactCount == 0)
                        {
                            connectedBody = collision.rigidbody;
                        }
                    }
                    if (desiresClimbing && upDot >= minClimbDotProduct && LayerMaskUtils.HasLayer(climbMask, layer))
                    {
                        climbContactCount += 1;
                        climbNormal += normal;
                        lastClimbNormal = normal;
                        if (LayerMaskUtils.HasLayer(climbMask, layer))
                        {
                            currentLadder = collision.gameObject.transform.parent.gameObject.GetComponent<Ladder>();
                        }
                        connectedBody = collision.rigidbody;
                    }
                }
            }
        }

        void UpdateConnectionState()
        {
            if (connectedBody == previousConnectedBody)
            {
                Vector3 connection_movement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
                connectionVelocity = connection_movement / Time.deltaTime;
            }
            connectionWorldPosition = body.position;
            connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
        }

        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }

        float GetMinDot(int layer)
        {
            return LayerMaskUtils.HasLayer(stairsMask, layer) ? minStairsDotProduct : minGroundDotProduct;
        }

        public void PreventSnapToGround()
        {
            stepsSinceLastJump = -1;
        }

        public void TransformToSpirit()
        {
            if (PlayerTrigger.instance.ObstacleDetected) return;
            StartCoroutine(AnimTurnToSpirit());
        }

        public void GoBackToNormalForm(bool spiritDead)
        {
            spiritParticles.Stop();

            if (spiritDead)
            {
                spiritDissolving = true;
                spiritDissolve.Dissolve();
            }
            else
            {
                ReturnToBody(false);
            }
        }

        public void ReturnToBody(bool instant)
        {
            if (!instant)
            {
                spiritCollider.isTrigger = true;

                spiritPosAtStartReturning = spiritObject.transform.localPosition;
                float distanceSpiritBody = Vector3.Distance(spiritPosAtStartReturning, Vector3.zero);
                timeForSpiritToReturn = distanceSpiritBody / spiritReturningAverageSpeed;
                timeSinceSpiritReturningStart = 0.0f;

                spiritDissolving = false;
                spiritReturning = true;
            }
            else
            {
                spiritDissolve.ForceNoDissolve();

                spiritObject.SetActive(false);
                spirit = false;

                if (disableSpiritTime > 0.0f)
                {
                    StartCoroutine(DisableSpirit());
                }
            }
        }

        public void FreezeSpirit()
        {
            spiritBody.velocity = Vector3.zero;
        }

        IEnumerator DisableSpirit()
        {
            spiritDisabled = true;
            yield return new WaitForSeconds(disableSpiritTime);
            spiritDisabled = false;
        }

        IEnumerator AnimTurnToSpirit()
        {
            disableCommands = true;
            animator.SetTrigger("ActivateSpirit");
            yield return new WaitForSeconds(0.1f);
            int activate_spirit_anim_id = animator.GetCurrentAnimatorClipInfo(0)[0].clip.GetInstanceID();
            yield return new WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.GetInstanceID() != activate_spirit_anim_id);
            disableCommands = false;

            spirit = true;
            spiritObject.SetActive(true);
            spiritObject.transform.localPosition = Vector3.zero;
            spiritObject.transform.localRotation = transform.rotation;
            spiritParticles.Play();
        }

        public Transform FocusPoint()
        {
            return spirit ? spiritObject.transform : transform;
        }

        public void SaveState()
        {
            savedPosition = transform.position;
            savedRotationModelFlip = modelFlip.localRotation;
        }

        public void RestoreState()
        {
            body.velocity = Vector3.zero;
            transform.position = savedPosition;
            modelFlip.localRotation = savedRotationModelFlip;
        }

        public void AnimTriggerLever()
        {
            animator.SetTrigger("TriggerLever");
        }
    }
}
