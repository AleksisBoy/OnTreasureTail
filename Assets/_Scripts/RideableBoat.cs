using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideableBoat : Interactable, IInteractable
{
    // TODO: FIX COLLISION WHEN BOAT BECOMES UNRIDDEN AND IT STILL GOES INTO TERRAIN

    [Header("Boat")]
    [SerializeField] private Transform frontPosition = null;
    [SerializeField] private Transform rearPosition = null;
    [SerializeField] private Transform frontHullPosition = null;
    [SerializeField] private Transform seatTransform = null;
    [SerializeField] private Transform leftPaddle = null;
    [SerializeField] private Transform rightPaddle = null;
    [SerializeField] private AnimationCurve movePattern = null;
    [SerializeField] private float patternSeconds = 2f;
    [SerializeField] private float rowingSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float decelerationRate = 10f;
    [SerializeField] private float kickOffSpeed = 5f;
    [SerializeField] private float parkHeight = 0.05f;
    [SerializeField] private float parkedRotationX = -5f;
    [SerializeField] private Vector3 parkedOffset = Vector3.zero;

    public Transform SeatTransform => seatTransform;
    private Vector3 velocity = Vector3.zero;
    private float time01 = 0f;
    private bool ridden = false;
    private bool parked = false;
    private bool kickOff = false;
    private void Start()
    {
        AssignToIslandManager();
        Park();
    }
    private void Update()
    {
        RiddenInput();

        UpdateVelocityPosition();
    }

    private void UpdateVelocityPosition()
    {
        if (velocity != Vector3.zero)
        {
            transform.position = transform.position + velocity * Time.deltaTime;
            velocity = Vector3.Lerp(velocity, Vector3.zero, decelerationRate * Time.deltaTime);
        }
    }

    private void RiddenInput()
    {
        if (!ridden) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0f && !parked)
        {
            transform.Rotate(0f, horizontalInput * rotationSpeed * Time.deltaTime, 0f);
        }

        if (verticalInput > 0f && !kickOff && !parked)
        {
            time01 += Time.deltaTime;
            if (time01 > 1f) time01 = 0f;
            velocity = transform.forward * rowingSpeed * movePattern.Evaluate(time01);
            PlayerInteraction.Instance.Animator.SetFloat("Rowing", Mathf.Abs(1f - movePattern.Evaluate(time01)));
        }
        else
        {
            time01 = 0f;
            PlayerInteraction.Instance.Animator.SetFloat("Rowing", 0f);
        }

        if (kickOff) BreakKickOffCheck();

        if (parked)
        {
            if (!kickOff && verticalInput < 0f) Unpark();
            return;
        }

        TerrainCollisionCheck();
    }

    private void BreakKickOffCheck()
    {
        float velocityMagnitude = velocity.magnitude;
        if (velocityMagnitude < 1f)
        {
            kickOff = false;
            parked = false;
        }
    }

    private void TerrainCollisionCheck()
    {
        Vector3 moveOffset = velocity * Time.deltaTime;
        Terrain terrain = IslandManager.Instance.Terrain;

        // Kick off if hit the back of the boat
        float rearHeight = terrain.SampleHeight(rearPosition.position + moveOffset) + terrain.transform.position.y;
        if (rearHeight > 0f)
        {
            KickOff(transform.forward * kickOffSpeed);
            return;
        }

        // Get the height of terrain on front hull position
        float groundHeight = terrain.SampleHeight(frontHullPosition.position + moveOffset) + terrain.transform.position.y;

        // Update the height of terrain with front position if hull was hit on terrain
        if (groundHeight > 0f) groundHeight = terrain.SampleHeight(frontPosition.position + moveOffset) + terrain.transform.position.y;
        else return;

        if (groundHeight > 0f && groundHeight <= parkHeight)
        {
            Park();
        }
        else if (groundHeight > parkHeight)
        {
            KickOff(-velocity);
        }
    }
    private void Park()
    {
        parked = true;
        transform.Rotate(parkedRotationX, 0f, 0f);
        transform.position += parkedOffset;
        velocity = Vector3.zero;
    }
    private void Unpark()
    {
        transform.Rotate(-parkedRotationX, 0f, 0f);
        transform.position -= parkedOffset;
        KickOff();
    }
    private void KickOff()
    {
        kickOff = true;
        velocity = -transform.forward * kickOffSpeed;
    }
    private void KickOff(Vector3 kickoffVelocity)
    {
        kickOff = true;
        velocity = kickoffVelocity;
    }
    private void SetPaddleParent(PlayerInteraction player)
    {
        if (player)
        {
            leftPaddle.SetParent(player.LeftHand, false);
            leftPaddle.transform.localPosition = Vector3.zero;
            leftPaddle.Rotate(20f, -100f, 0f);
            rightPaddle.SetParent(player.RightHand, false);
            rightPaddle.transform.localPosition = Vector3.zero;
            rightPaddle.Rotate(20f, 100f, 0f);
        }
        else
        {
            leftPaddle.SetParent(transform.parent, false);
            rightPaddle.SetParent(transform.parent, false);
            leftPaddle.rotation = Quaternion.identity;
            rightPaddle.rotation = Quaternion.identity;
        }
    }
    // IInteractable calls
    public void AssignToIslandManager()
    {
        IslandManager.Instance.AssignInteractable(this);
    }

    public Vector3 GetInteractionPosition()
    {
        return interactionPoint.position;
    }

    public bool HasInteracted()
    {
        return false;
    }

    public void Interact()
    {
        if (kickOff) return;

        if (ridden)
        {
            // go off the boat
            PlayerInteraction.Instance.RideBoat(null);
            SetPaddleParent(null);
            ridden = false;
        }
        else
        {
            // enter boat
            if (parked)
            {
                Unpark();
            }
            PlayerInteraction.Instance.RideBoat(this);
            SetPaddleParent(PlayerInteraction.Instance);
            ridden = true;
        }
    }
    public bool InteractionActive()
    {
        return gameObject.activeSelf;
    }

    public float GetInteractionDistance()
    {
        return interactionDistance;
    }
}
