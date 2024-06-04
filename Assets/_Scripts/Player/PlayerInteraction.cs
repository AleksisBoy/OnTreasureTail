using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform rightHandTransform = null;
    [SerializeField] private Transform leftHandTransform = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private PlayerMovement movement = null;
    [SerializeField] private PlayerCamera view = null;
    [SerializeField] private Camera focusCamera = null;
    [Header("Digging")]
    [SerializeField] private float diggingRadius = 0.5f;
    [SerializeField] private float diggingHeightDelta = -0.0001f;
    [SerializeField] private float diggingForwardModifier = 0.3f;

    private Terrain terrain = null;
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
    }
    private void Start()
    {
        terrain = IslandManager.Instance.Terrain;
        movement.Set(terrain, animator);
    }
    private void Update()
    {
        InteractionInput();
        DiggingInput();
    }

    private void DiggingInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int terrainLayer = InternalSettings.GetMainTexture(terrain, transform.position);
            if (terrainLayer == IslandManager.Instance.SandLayerIndex)
            {
                DigInFront();
            }
        }
    }

    private void DigInFront()
    {
        Vector3 diggingPos = transform.position + transform.forward * diggingForwardModifier;
        Collider[] colls = Physics.OverlapSphere(diggingPos, diggingRadius, InternalSettings.Get.EnvironmentMask);
        if(colls.Length > 0 )
        {
            Debug.Log("Blocking digging");
            return;
        }
        if (IslandManager.Instance.CanDigAt(diggingPos, diggingRadius))
        {
            animator.SetBool("Dig", true);
        }
    }
    public void AnimationEvent_DigImpact()
    {
        Vector3 diggingPos = transform.position + transform.forward * diggingForwardModifier;
        IslandManager.Instance.DigIslandTerrain(diggingPos, diggingRadius, diggingHeightDelta);
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
    public Animator Animator => animator;
    public Transform RightHand => rightHandTransform;
    public Transform LeftHand => leftHandTransform;
}
