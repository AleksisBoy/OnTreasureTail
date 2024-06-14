using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerRadius = 0.25f;
    [SerializeField] private Transform rightFootTransform = null;
    [SerializeField] private Transform leftFootTransform = null;
    [SerializeField] private float footstepCooldown = 0.1f;
    [Header("Grounded")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float slopeOffsetX = 20f;
    [SerializeField] private float slopeModifier = 0.1f;
    [SerializeField] private float slideSpeed = 4f;
    [SerializeField] private float glideModifier = 0.05f;
    [SerializeField] private Vector3 collisionOffset = Vector3.zero;
    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 5f; 
    [SerializeField] private float jumpDistance = 2f; 
    [SerializeField] private float jumpHeight = 1.5f; 
    [SerializeField] private AnimationCurve jumpCurve = null;
    [Header("Swimming")]
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float swimSprintSpeed = 5f;
    [SerializeField] private float waterLevel = 0f;

    private Animator animator;
    private Terrain terrain;
    private PlayerCombat combat;
    private PlayerCamera playerCamera;

    private bool sloping = false; 
    private bool grounded = false; 
    private bool jumping = false; 
    private bool evading = false;

    private float process = 0f; 
    private Vector3 lastPosition = Vector3.zero;
    public float velocityFloat = 0f;
    private float lastFootstepTime = 0f;
    public bool Grounded => grounded;
    public void Set(Terrain terrain, Animator animator, PlayerCamera playerCamera)
    {
        this.terrain = terrain;
        this.animator = animator;
        this.playerCamera = playerCamera;

        lastPosition = transform.position;

        if (Physics.Raycast(transform.position + new Vector3(0f, 0.1f, 0f), Vector3.down, 1f))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Sloping", sloping);
    }
    public Vector3 GetCameraDirectionFromInput(float horizontalInput, float verticalInput)
    {
        Vector3 cameraDirection = playerCamera.transform.TransformDirection(new Vector3(horizontalInput, 0f, verticalInput));
        cameraDirection.y = 0f;
        return cameraDirection.normalized;
    }
    private void Update()
    {
        if (animator.GetBool("Dig")) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        animator.SetFloat("InputX", horizontalInput);
        animator.SetFloat("InputY", verticalInput);
        bool inCombat = combat.InCombat();

        Vector3 cameraDirection = GetCameraDirectionFromInput(horizontalInput, verticalInput);
        RotationInput(cameraDirection, horizontalInput, verticalInput, inCombat);

        if (!inCombat && JumpInput()) return;
        if (inCombat && EvadeInput(horizontalInput, verticalInput, cameraDirection)) return;

        // Get the speed depending on input
        float currentSpeed = GetCurrentSpeedInput();
        float maxSpeed;

        // Create modifier for limiting movement speed in case of both inputs active or sloping active
        //float speedModifier = horizontalInput != 0f && verticalInput != 0f ? 0.71f : 1f;
        float speedModifier = 1f;
        if (sloping) speedModifier *= slopeModifier;

        // Getting the next position wanted
        Vector3 desiredPosition = new Vector3(
            transform.position.x + cameraDirection.x * currentSpeed * Time.deltaTime * speedModifier,
            0f,
            transform.position.z + cameraDirection.z * currentSpeed * Time.deltaTime * speedModifier);

        // Checking desiredPosition
        if (Physics.Raycast(desiredPosition + new Vector3(0f, transform.position.y + 0.1f, 0f), Vector3.down, out RaycastHit hit, 1f) && hit.point.y > waterLevel)
        {
            // Desired to be on ground
            grounded = true;
            maxSpeed = runSpeed;

            desiredPosition.y = hit.point.y;
            //desiredPosition.y = terrain.SampleHeight(desiredPosition) + terrain.transform.position.y;

            // Setting grounded position for player with slope consideration
            if (!SlopeBehaviour(hit.normal))
            {
                transform.position = DesiredPositionWithCollision(desiredPosition);
            }
        }
        else
        {
            // Desired to be in water
            grounded = false;
            maxSpeed = swimSprintSpeed;
            sloping = false;
            desiredPosition.y = 0f;
            transform.position = DesiredPositionWithCollision(desiredPosition);
        }

        // Set height of player position
        transform.position = new Vector3(transform.position.x, desiredPosition.y, transform.position.z);

        // Velocity calculation
        velocityFloat = ((transform.position - lastPosition) / Time.deltaTime).magnitude;
        lastPosition = transform.position;

        // Update Animations
        animator.SetFloat("Speed", velocityFloat / maxSpeed);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Sloping", sloping);
    }

    private void RotationInput(Vector3 forwardDirection, float horizontalInput, float verticalInput, bool inCombat)
    {
        // Yaw rotation of player
        if (horizontalInput != 0f || verticalInput != 0f)
        {
            Quaternion lookRotation;
            if (inCombat && !evading)
            {
                lookRotation = Quaternion.LookRotation((TailUtil.PositionFlat(combat.Target.transform.position) - TailUtil.PositionFlat(transform.position)).normalized, Vector3.up);
            }
            else
            {
                lookRotation = Quaternion.LookRotation(TailUtil.PositionFlatNormalized(forwardDirection), Vector3.up);
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private bool JumpInput()
    {
        // add in air slight movement
        if (Input.GetKeyDown(KeyCode.Space) && !jumping && grounded && velocityFloat > 0f)
        {
            jumping = true;
            process = 0f;
            // check for collision
            Vector3 direction = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Vector3 jumpFinalPosition =  new Vector3(transform.position.x + direction.x * (1f /jumpSpeed)  * jumpDistance, 0f, transform.position.z + direction.z * (1f / jumpSpeed) * jumpDistance);
            jumpFinalPosition.y = terrain.SampleHeight(jumpFinalPosition) + terrain.transform.position.y;
            if (jumpFinalPosition.y < waterLevel)
            {
                // jumped in water
                animator.SetTrigger("JumpInWater");
            }
            else
            {
                Collider[] colls = Physics.OverlapSphere(jumpFinalPosition, playerRadius, InternalSettings.Get.EnvironmentMask);
                if (colls.Length > 0)
                {
                    Debug.Log("jumped on obstacle");
                    jumping = false;
                }
            }
        }
        if (jumping)
        {
            Vector3 direction = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Vector3 jumpPosition = new Vector3(transform.position.x + direction.x * Time.deltaTime * jumpDistance, 0f, transform.position.z + direction.z * Time.deltaTime * jumpDistance);
            transform.position = new Vector3(jumpPosition.x, terrain.SampleHeight(jumpPosition) + jumpCurve.Evaluate(process) * jumpHeight + terrain.transform.position.y, jumpPosition.z);
            
            process += jumpSpeed * Time.deltaTime;
            if (process > 1f)
            {
                jumping = false;
            }
        }
        return jumping;
    }
    private bool EvadeInput(float horizontalInput, float verticalInput, Vector3 forwardDirection)
    {
        if ((horizontalInput == 0f && verticalInput == 0f))
        {
            evading = false;
            return false;
        }

        if (!evading && grounded && Input.GetKeyDown(KeyCode.Space) && combat.TryEvade())
        {
            evading = true;
            process = 0f;
            animator.SetTrigger("Evade");
        }
        if (evading)
        {
            Vector3 evadePosition = new Vector3(transform.position.x + forwardDirection.x * Time.deltaTime * combat.EvadeDistance, 0f, transform.position.z + forwardDirection.z * Time.deltaTime * combat.EvadeDistance);
            transform.position = DesiredPositionWithCollision(new Vector3(evadePosition.x, terrain.SampleHeight(evadePosition)  + terrain.transform.position.y, evadePosition.z));

            process += combat.EvadeSpeed * Time.deltaTime;
            if (process > 1f)
            {
                evading = false;
            }
        }
        return evading;
    }
    private bool SlopeBehaviour(Vector3 hitNormal)
    {
        float offsetCurrent = sloping ? slopeOffsetX - 10f : slopeOffsetX;
        
        if (Vector3.Angle(Vector3.up, hitNormal) > offsetCurrent)
        {
            sloping = true;
            Vector3 direction = new Vector3(hitNormal.x, 0f, hitNormal.z);
            direction.Normalize();
            transform.position = DesiredPositionWithCollision(transform.position + direction * Time.deltaTime * slideSpeed);
        }
        else
        {
            sloping = false;
        }
        return sloping;
    }
    private Vector3 DesiredPositionWithCollision(Vector3 desiredPosition)
    {
        // Update new position with collision check
        Vector3 direction = (desiredPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, desiredPosition);
        if (Physics.SphereCast(transform.position + new Vector3(0f, playerRadius, 0f), playerRadius, direction, out RaycastHit mainHit, distance, InternalSettings.Get.EnvironmentMask))
        {
            // Get the value that indicates the facing to the collision (0 - facing in front, 1 - facing sideways)
            float dot = 1f + Vector3.Dot(direction, mainHit.normal);

            // Get the direction of the wall
            Vector3 wallDirection = Vector3.Cross(Vector3.up, mainHit.normal);
            Vector3 cross = Vector3.Cross(transform.forward, (mainHit.point - transform.position).normalized);
            if(cross.y < 0f)
            {
                wallDirection = -wallDirection;
            }

            // Get the movement offset sliding on the wall
            Vector3 sideOffset = wallDirection * dot * glideModifier;

            // Check if side movement does not hit other obstacles
            if (Physics.SphereCast(transform.position + collisionOffset, playerRadius, wallDirection, out RaycastHit sideHit, sideOffset.magnitude, InternalSettings.Get.EnvironmentMask))
            {
                // Do not move if there is an obstacle to the side
                desiredPosition = transform.position;
            }
            else
            {
                // Move if no obstacles to the side
                desiredPosition = transform.position + sideOffset;
            }
        }
        else
        {
            // Next position is inside environment object
            Collider[] colls = Physics.OverlapSphere(desiredPosition, playerRadius, LayerMask.NameToLayer("Environment"));

            if (colls.Length > 0) desiredPosition = transform.position;
        }
        return desiredPosition;
    }
    private float GetCurrentSpeedInput()
    {
        // Setting current speed depending on grounded
        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed;
        if (combat.InCombat())
        {
            currentSpeed = sprinting ? combat.SprintSpeed : combat.Speed;
        }
        else if (grounded)
        {
            currentSpeed = sprinting ? runSpeed : walkSpeed;
        }
        else 
        {
            currentSpeed = sprinting ? swimSprintSpeed : swimSpeed;
        }
        return currentSpeed;
    }
    public void SetCombat(PlayerCombat combat)
    {
        this.combat = combat;
    }
    private void Footstep(Transform footTransform)
    {
        if (Time.time - lastFootstepTime < footstepCooldown) return;
        // check if the grounf is suitable for creating footstep

        if (!Physics.Raycast(footTransform.position, Vector3.down, out RaycastHit hit, 10f, InternalSettings.Get.TerrainMask)) return;

        Vector3 groundPosition = TailUtil.PositionFlat(footTransform.position);
        groundPosition.y = terrain.SampleHeight(groundPosition) + terrain.transform.position.y;

        GameObject footstep = ObjectPoolingManager.GetObject(InternalSettings.Get.FootStepPrefab, groundPosition, Quaternion.identity);

        footstep.transform.up = hit.normal;
        lastFootstepTime = Time.time;
    }
    // Animation
    public void AnimationEvent_FootstepRight()
    {
        Footstep(rightFootTransform);
    }
    public void AnimationEvent_FootstepLeft()
    {
        Footstep(leftFootTransform);
    }
    private void OnDisable()
    {
        animator.SetFloat("Speed", 0f);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerRadius);
    }
}
