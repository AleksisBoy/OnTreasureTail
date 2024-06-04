using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform rightHandTransform = null;
    [SerializeField] private Transform leftHandTransform = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private Terrain terrain = null;
    [SerializeField] private PlayerMovement movement = null;
    [SerializeField] private PlayerCamera view = null;
    [SerializeField] private Camera focusCamera = null;

    private RideableBoat boat = null;
    public static PlayerInteraction Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        focusCamera.gameObject.SetActive(false);
        movement.Set(terrain, animator);
    }
    private void Update()
    {
        InteractionInput();
    }

    private void InteractionInput()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        IInteractable inter = IslandManager.Instance.GetClosestInteractable(transform.position, out float distance);

        if (inter != null && distance < inter.GetInteractionDistance())
        {
            inter.Interact();
        }
    }
    public void RideBoat(RideableBoat boat)
    {
        this.boat = boat;
        bool rideBoat = boat != null;

        if (rideBoat)
        {
            transform.SetParent(boat.SeatTransform, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            animator.SetLayerWeight(2, 1f);
            animator.SetLayerWeight(0, 0f);
        }
        else
        {
            transform.SetParent(null, true);
            animator.SetLayerWeight(2, 0f);
            animator.SetLayerWeight(0, 1f);
        }
        movement.enabled = !rideBoat;
    }
    public void EnablePlayerComponents(bool state)
    {
        movement.enabled = state;
        view.enabled = state;
        this.enabled = state;
    }
    public Camera GetFocusCamera()
    {
        return focusCamera;
    }
    public PlayerCamera GetPlayerCamera()
    {
        return view;
    }
    public PlayerMovement Movement => movement;
    public Terrain Terrain => terrain;
    public Animator Animator => animator;
    public Transform RightHand => rightHandTransform;
    public Transform LeftHand => leftHandTransform;
}
