using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Terrain terrain = null;
    [SerializeField] private Animator animator = null;
    [Header("Grounded")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float slopeOffsetX = 20f;
    [SerializeField] private float slopeModifier = 0.1f;
    [SerializeField] private float slideSpeed = 4f;
    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 5f; 
    [SerializeField] private float jumpDistance = 2f; 
    [SerializeField] private float jumpHeight = 1.5f; 
    [SerializeField] private AnimationCurve jumpCurve = null;
    [Header("Swimming")]
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float swimSprintSpeed = 5f;

    private bool sloping = false; 
    private bool grounded = false; 
    private bool jumping = false; 
    private float jump = 0f; 
    private Vector3 lastPosition = Vector3.zero;
    public float velocityFloat = 0f; 
    private void Start()
    {
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
    }
    [ContextMenu("Print Rotation")]
    public void PrintRotation()
    {
        Debug.Log(transform.eulerAngles);
    }
    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (JumpInput()) return;

        // Yaw rotation of player
        if (horizontalInput != 0f || verticalInput != 0f) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(horizontalInput, 0f, verticalInput), Vector3.up), Time.deltaTime * rotationSpeed);

        // Get the speed depending on input
        float currentSpeed = GetCurrentSpeedInput();
        float maxSpeed;

        // Create modifier for limiting movement speed in case of both inputs active or sloping active
        float speedModifier = horizontalInput != 0f && verticalInput != 0f ? 0.71f : 1f;
        if (sloping) speedModifier *= slopeModifier;

        // Getting the next position wanted
        Vector3 desiredPosition = new Vector3(
            transform.position.x + horizontalInput * currentSpeed * Time.deltaTime * speedModifier,
            0f,
            transform.position.z + verticalInput * currentSpeed * Time.deltaTime * speedModifier);

        // Checking desiredPosition
        if(Physics.Raycast(desiredPosition + new Vector3(0f, transform.position.y + 0.1f, 0f), Vector3.down, out RaycastHit hit, 1f) && hit.point.y > 0f)
        {
            // Desired to be on ground
            grounded = true;
            maxSpeed = runSpeed;

            desiredPosition.y = terrain.SampleHeight(desiredPosition) + terrain.transform.position.y;

            // Setting grounded position for player with slope consideration
            if (!SlopeBehaviour(hit.normal))
            {
                transform.position = desiredPosition;
            }
        }
        else
        {
            // Desired to be in water
            grounded = false;
            maxSpeed = swimSprintSpeed;
            sloping = false;
            desiredPosition.y = 0f;
            transform.position = desiredPosition;
        }

        // Set height of player position
        transform.position = new Vector3(transform.position.x, desiredPosition.y, transform.position.z);

        // Velocity calculation
        velocityFloat = ((transform.position - lastPosition) / Time.deltaTime).magnitude;
        lastPosition = transform.position;

        // Update Animations
        animator.SetFloat("Speed", velocityFloat / maxSpeed);
        animator.SetBool("Grounded", grounded);
    }
    private bool JumpInput()
    {
        // add in air slight movement
        if (Input.GetKeyDown(KeyCode.Space) && !jumping && grounded)
        {
            jumping = true;
            jump = 0f;
        }
        if (jumping)
        {
            Vector3 direction = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Vector3 jumpPosition = new Vector3(transform.position.x + direction.x * Time.deltaTime * jumpDistance, 0f, transform.position.z + direction.z * Time.deltaTime * jumpDistance);
            transform.position = new Vector3(jumpPosition.x, terrain.SampleHeight(jumpPosition) + jumpCurve.Evaluate(jump) * jumpHeight + terrain.transform.position.y, jumpPosition.z);
            
            jump += jumpSpeed * Time.deltaTime;
            if (jump > 1f)
            {
                jumping = false;
            }
        }
        return jumping;
    }
    private bool SlopeBehaviour(Vector3 hitNormal)
    {
        float offsetCurrent = sloping ? slopeOffsetX - 10f : slopeOffsetX;
        
        if (Vector3.Angle(Vector3.up, hitNormal) > offsetCurrent)
        {
            sloping = true;
            Vector3 direction = new Vector3(hitNormal.x, 0f, hitNormal.z);
            direction.Normalize();
            transform.position += direction * Time.deltaTime * slideSpeed;
        }
        else
        {
            sloping = false;
        }
        return sloping;
    }
    private float GetCurrentSpeedInput()
    {
        // Setting current speed depending on grounded
        float currentSpeed;
        if (grounded)
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }
        else 
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? swimSprintSpeed : swimSpeed;
        }
        return currentSpeed;
    }
}
