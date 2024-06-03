using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideableBoat : Interactable, IInteractable
{
    [Header("Boat")]
    [SerializeField] private Transform seatTransform = null;
    [SerializeField] private AnimationCurve movePattern = null;
    [SerializeField] private float patternSeconds = 2f;
    [SerializeField] private float rowingSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float decelerationRate = 10f;

    public Transform SeatTransform => seatTransform;
    private Vector3 velocity = Vector3.zero;
    private float time01 = 0f;
    private bool ridden = false;
    private void Start()
    {
        AssignToIslandManager();
    }
    private void Update()
    {
        if (!ridden) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 cameraDirection = PlayerInteraction.Instance.Movement.GetCameraDirectionFromInput(horizontalInput, verticalInput);
        if (horizontalInput != 0f || verticalInput != 0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(cameraDirection.x, 0f, cameraDirection.z), Vector3.up), Time.deltaTime * rotationSpeed);
            time01 += Time.deltaTime;
            if (time01 > 1f) time01 = 0f;
            float speedModifier = horizontalInput != 0f && verticalInput != 0f ? 0.71f : 1f;
            velocity = transform.forward * rowingSpeed * movePattern.Evaluate(time01) * speedModifier;
        }
        else
        {
            time01 = 0f;
            velocity = Vector3.Lerp(velocity, Vector3.zero, decelerationRate * Time.deltaTime);
        }
        transform.position += velocity * Time.deltaTime;
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
        if (ridden)
        {
            // go off the boat
            PlayerInteraction.Instance.RideBoat(null);
        }
        else
        {
            // enter boat
            PlayerInteraction.Instance.RideBoat(this);
        }
        ridden = !ridden;
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
