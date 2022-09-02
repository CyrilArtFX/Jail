using System.Collections;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [Header("Solid Body")]
    [SerializeField] Transform modelFlip = default;

    [Header("Spirit")]
    [SerializeField] GameObject spiritObject = default;
    [SerializeField] Transform spiritModelFlip = default;

    [Header("Parameters")]
    [SerializeField, Range(0f, 100f)] float maxSpeed = 13f, maxClimbSpeed = 2f, maxSpiritSpeed = 8f, maxSlowSpeed = 1f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f, maxAirAcceleration = 1f, maxClimbAcceleration = 20f, maxSpiritAcceleration = 15f;
    [SerializeField, Range(0f, 10f)] float jumpHeight = 2f;
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f, maxStairsAngle = 50f;
    [SerializeField, Range(0f, 100f)] float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)] float probeDistance = 1f;
    [SerializeField, Range(90f, 170f)] float maxClimbAngle = 140f;
    [SerializeField] LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, ladderMask = 0;
    [SerializeField, Min(0f)] float modelAlignSpeed = 180f;

    [Header("Others")]
    [SerializeField] bool maintainButtonForClimb = false;
    [SerializeField] Transform camFocus = default;

    [Header("UI")]
    [SerializeField] GameObject deathText = default;

    Rigidbody body, connectedBody, previousConnectedBody;
    Animator animator;

    Vector2 playerInput;
    Vector3 velocity, connectionVelocity;
    Vector3 groundNormal, contactNormal, steepNormal, climbNormal, lastClimbNormal;
    Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;
    int groundContactCount, steepContactCount, climbContactCount;
    Ladder currentLadder;
    bool OnGround => groundContactCount > 0;
    bool OnSteep => steepContactCount > 0;
    bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;
    bool desiredJump, desiresClimbing, requestClimbing;
    bool spirit = false, spiritReturning = false;
    float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    int stepsSinceLastGrounded, stepsSinceLastJump, stepsSinceLastClimbRequest;
    Vector3 connectionWorldPosition, connectionLocalPosition;
    Vector3 startPosition;
    Quaternion startRotationModelFlip;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        deathText.SetActive(false);
        startPosition = transform.position;
        startRotationModelFlip = modelFlip.localRotation;

        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        body.useGravity = false;
        spiritObject.GetComponent<Rigidbody>().useGravity = false;
        spiritObject.SetActive(false);
        OnValidate();
    }

    void Update()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Climbing || spirit ? Input.GetAxis("UpDown") : 0f;
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);

        if (maintainButtonForClimb) desiresClimbing = Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f;
        else
        {
            desiresClimbing |= Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f;
            requestClimbing = Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f;
        }
            
        desiredJump |= Input.GetButtonDown("Jump");

        if (spirit)
        {
            desiresClimbing = false;
            requestClimbing = false;
            desiredJump = false;
            camFocus.localPosition = spiritObject.transform.localPosition;
        }
        else camFocus.localPosition = Vector3.zero;

        UpdateRotations();

        animator.SetBool("Climb", Climbing);
    }

    void UpdateRotations()
    {
        if (spirit) //Make the spirit head towards where he goes
        {
            Vector3 spiritRotationPlaneNormal;
            if (spiritObject.GetComponent<Rigidbody>().velocity == Vector3.zero || playerInput == Vector2.zero && !spiritReturning) spiritRotationPlaneNormal = spiritObject.transform.up;
            else spiritRotationPlaneNormal = spiritObject.GetComponent<Rigidbody>().velocity.normalized;

            Quaternion spiritRotation = spiritObject.transform.localRotation;
            if (modelAlignSpeed > 0f)
            {
                spiritRotation = AlignModelRotation(spiritRotationPlaneNormal, spiritRotation);
            }
            spiritObject.transform.localRotation = spiritRotation;
        }


        Quaternion flipRotation = modelFlip.localRotation; //Make the spirit or the player face toward where he goes
        if(playerInput.x != 0 && !Climbing)
        {
            int rotationFactor = 500;

            float secondRotationY = flipRotation.eulerAngles.y;
            secondRotationY = Mathf.Clamp(secondRotationY - playerInput.x * rotationFactor * Time.deltaTime, 0, 180);
            flipRotation = Quaternion.Euler(0, secondRotationY, 0);
        }
        else
        {
            if(Climbing)
            {
                if (currentLadder != null) flipRotation = Quaternion.Euler(0, 270, 0);
            }
        }
        if(Time.timeScale != 0)
        {
            if (spirit) spiritModelFlip.localRotation = flipRotation;
            else modelFlip.localRotation = flipRotation;
        }
    }

    Quaternion AlignModelRotation(Vector3 rotationAxis, Quaternion rotation)
    {
        Vector3 modelAxis = spiritObject.transform.up;
        float dot = Mathf.Clamp(Vector3.Dot(modelAxis, rotationAxis), -1f, 1f);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        float maxAngle = modelAlignSpeed * Time.deltaTime;

        Quaternion newAlignment = Quaternion.FromToRotation(modelAxis, rotationAxis) * rotation;
        if (angle <= maxAngle || Climbing) return newAlignment;
        else
        {
            return Quaternion.SlerpUnclamped(rotation, newAlignment, maxAngle / angle);
        }
    }

    void FixedUpdate()
    {
        UpdateState();

        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        Vector3 bodyVelocity = velocity;
        
        if(spirit)
        {
            spiritObject.GetComponent<Rigidbody>().velocity = velocity;
            bodyVelocity = Vector3.zero;
        }


        if (Climbing)
        {
            bodyVelocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
        }
        else if(OnGround && velocity.sqrMagnitude < 0.01f)
        {
            bodyVelocity += contactNormal * (Vector3.Dot(Physics.gravity, contactNormal) * Time.deltaTime);
        }
        else if(desiresClimbing && OnGround)
        {
            bodyVelocity += (Physics.gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
        }
        else
        {
            bodyVelocity += Physics.gravity * Time.deltaTime;
        }

        body.velocity = bodyVelocity;

        ClearState();
    }

    void ClearState()
    {
        lastContactNormal = contactNormal;
        lastSteepNormal = steepNormal;
        lastConnectionVelocity = connectionVelocity;
        groundContactCount = steepContactCount = climbContactCount = 0;
        groundNormal = contactNormal = steepNormal = connectionVelocity = climbNormal = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
    }

    void UpdateState()
    {
        stepsSinceLastGrounded++;
        stepsSinceLastJump++;
        stepsSinceLastClimbRequest = requestClimbing ? 0 : stepsSinceLastClimbRequest + 1;
        velocity = spirit ? spiritObject.GetComponent<Rigidbody>().velocity : body.velocity;
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
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass) UpdateConnectionState();
        }
    }

    void AdjustVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis;
        if(spirit)
        {
            acceleration = maxSpiritAcceleration;
            speed = maxSpiritSpeed;
            xAxis = Vector3.back;
        }
        else if(Climbing)
        {
            acceleration = maxClimbAcceleration;
            speed = maxClimbSpeed;
            xAxis = currentLadder != null ? Vector3.back : Vector3.zero;
        }
        else
        {
            acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
            speed = desiresClimbing ? maxSlowSpeed : maxSpeed;
            xAxis = Vector3.back;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector2 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.y = Climbing || spirit ? playerInput.y * speed - Vector3.Dot(relativeVelocity, Vector3.up) : 0f;
        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        velocity += xAxis * adjustment.x;

        if(Climbing || spirit)
        {
            velocity += Vector3.up * adjustment.y;
        }
        if(spiritReturning)
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
        float upDot = Vector3.Dot(Vector3.up, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }

    void Jump()
    {
        Vector3 jumpDirection;
        if (OnGround)
        {
            jumpDirection = contactNormal;
            if(Climbing && currentLadder != null)
            {
                jumpDirection = Vector3.zero;
                currentLadder.DesactiveClimbable();
                currentLadder = null;
            }
        }
        else if (OnSteep)
        {
            jumpDirection = steepNormal;
        }
        else return;

        stepsSinceLastJump = 0;
        float jumpSpeed = Mathf.Sqrt(2f * Physics.gravity.magnitude * jumpHeight);
        jumpDirection = (jumpDirection + Vector3.up).normalized;
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        if (alignedSpeed > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += jumpDirection * jumpSpeed;
    }

    bool CheckSteepContact()
    {
        if(steepContactCount > 1)
        {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(Vector3.up, steepNormal);
            if(upDot >= minGroundDotProduct)
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
        if(Climbing)
        {
            if (stepsSinceLastClimbRequest > 10 && groundNormal != Vector3.zero) desiresClimbing = false;
            if (climbContactCount > 1)
            {
                climbNormal.Normalize();
                float upDot = Vector3.Dot(Vector3.up, climbNormal);
                if (upDot >= minGroundDotProduct) climbNormal = lastClimbNormal;
            }
            groundContactCount = 1;
            contactNormal = climbNormal;
            return true;
        }
        else
        {
            if (!maintainButtonForClimb && !requestClimbing && stepsSinceLastClimbRequest > 2 && climbNormal == Vector3.zero) desiresClimbing = false;
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
        float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(Vector3.up, normal);
            if (upDot >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
                groundNormal += normal;
            }
            else
            {
                if(upDot > -0.01f)
                {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0) connectedBody = collision.rigidbody;
                }
                if(desiresClimbing && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) != 0)
                {
                    climbContactCount += 1;
                    climbNormal += normal;
                    lastClimbNormal = normal;
                    if ((ladderMask & (1 << collision.gameObject.layer)) != 0) currentLadder = collision.gameObject.transform.parent.gameObject.GetComponent<Ladder>();
                    connectedBody = collision.rigidbody;
                }
            }
        }
    }

    void UpdateConnectionState()
    {
        if(connectedBody == previousConnectedBody)
        {
            Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
    }

    Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    float GetMinDot (int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }

    public void PreventSnapToGround()
    {
        stepsSinceLastJump = -1;
    }

    public void TransformToSpirit()
    {
        spirit = true;
        spiritObject.SetActive(true);
        spiritObject.transform.localPosition = Vector3.zero;
        spiritObject.transform.localRotation = transform.rotation;
    }

    public void GoBackToNormalForm()
    {
        StartCoroutine(ReturnToBody());
    }

    private IEnumerator ReturnToBody()
    {
        spiritReturning = true;
        spiritObject.GetComponent<CapsuleCollider>().isTrigger = true;
        while (Vector3.Distance(spiritObject.transform.localPosition, Vector3.zero) > 0.1f)
        {
            spiritObject.GetComponent<Rigidbody>().AddForce(-spiritObject.transform.localPosition.normalized * Time.deltaTime * 100000f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        spiritObject.GetComponent<CapsuleCollider>().isTrigger = false;
        spiritReturning = false;
        spiritObject.SetActive(false);
        spirit = false;
    }

    public Transform FocusPoint()
    {
        return spirit ? spiritObject.transform : transform;
    }

    public void Die()
    {
        StartCoroutine(DeathMessage());
    }

    private IEnumerator DeathMessage()
    {
        animator.SetBool("Slide", false);
        animator.SetBool("Crouch", false);
        animator.SetBool("Climb", false);
        spirit = false;
        spiritObject.SetActive(false);

        deathText.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitUntil(() => Input.anyKeyDown);

        body.velocity = Vector3.zero;
        transform.position = startPosition;
        modelFlip.localRotation = startRotationModelFlip;
        Time.timeScale = 1f;
        deathText.SetActive(false);
    }
}
