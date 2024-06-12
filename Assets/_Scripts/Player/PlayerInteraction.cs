using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform rightHandTransform = null;
    [SerializeField] private Transform leftHandTransform = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private PlayerMovement movement = null;
    [SerializeField] private PlayerCamera view = null;
    [SerializeField] private PlayerSubinteraction[] subinteractions = null;
    [SerializeField] private PlayerEquipment equipment = null;
    [SerializeField] private Health playerHealth = null;
    //[SerializeField] private PlayerCombat combat = null;

    private Terrain terrain = null;
    private RideableBoat boat = null;
    public Health Health => playerHealth;
    public static PlayerInteraction Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach(var sub in subinteractions)
        {
            sub.enabled = false;
            sub.Set(animator, movement, view);
        }
        equipment.AssignOnEquippedChanged(OnEqippedChanged);
    }
    private void OnEqippedChanged(ItemTail item)
    {
        string itemName = item != null ? item.ItemName : string.Empty;
        foreach(PlayerSubinteraction sub in subinteractions)
        {
            if (item != null && sub.ItemName == itemName && !sub.enabled)
            {
                sub.enabled = true;
                item.meshObject.transform.SetParent(rightHandTransform, false);
            }
            else if((item == null && sub.enabled) || (sub.ItemName != itemName && sub.enabled))
            {
                sub.enabled = false;
            }
        }
    }
    private void Start()
    {
        terrain = IslandManager.Instance.Terrain;
        movement.Set(terrain, animator, view);
    }
    private void Update()
    {
        InteractionInput();
    }
    private void FixedUpdate()
    {
        IInteractable inter = IslandManager.Instance.GetClosestInteractable(transform.position, out float distance);

        if (inter != null && distance < inter.GetInteractionDistance())
        {
            animator.SetBool("Interact", true);
        }
        else
        {

            animator.SetBool("Interact", false);
        }
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
            animator.SetLayerWeight(animator.GetLayerIndex("Boat"), 1f);
            animator.SetLayerWeight(animator.GetLayerIndex("Ground"), 0f);
        }
        else
        {
            transform.SetParent(null, true);
            animator.SetLayerWeight(animator.GetLayerIndex("Boat"), 0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Ground"), 1f);
        }
        movement.enabled = !rideBoat;
    }
    public void SetCombatAnimator(bool state)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Ground"), state ? 0f : 1f);
        animator.SetLayerWeight(animator.GetLayerIndex("Combat"), state ? 1f : 0f);
    }
    public void EnablePlayerComponents(bool state)
    {
        movement.enabled = state;
        view.enabled = state;
        this.enabled = state;
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
