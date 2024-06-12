using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDigging : PlayerSubinteraction
{
    [Header("Digging")]
    [SerializeField] private KeyCode digInput = KeyCode.F;
    [SerializeField] private float diggingRadius = 0.5f;
    [SerializeField] private float diggingHeightDelta = -0.00025f;
    [SerializeField] private float diggingForwardModifier = 0.6f;

    private Terrain terrain = null;

    // Set properties
    private PlayerMovement movement = null;

    private void Start()
    {
        terrain = IslandManager.Instance.Terrain;
    }
    public override void Set(Animator animator, params object[] setList)
    {
        base.Set(animator, setList);
        foreach (var item in setList)
        {
            if(item as PlayerMovement)
            {
                movement = (PlayerMovement)item;
            }
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    private void Update()
    {
        DiggingInput();
    }
    private void DiggingInput()
    {
        if (movement.Grounded && Input.GetKeyDown(digInput))
        {
            int terrainLayer = InternalSettings.GetMainTexture(terrain, transform.position);
            foreach (int layer in IslandManager.Instance.SandLayerIndex)
            {
                if (terrainLayer == layer)
                {
                    DigInFront();
                    return;
                }
            }
        }
    }
    private void DigInFront()
    {
        Vector3 diggingPos = transform.position + transform.forward * diggingForwardModifier;
        Collider[] colls = Physics.OverlapSphere(diggingPos, diggingRadius, InternalSettings.Get.EnvironmentMask);
        if (colls.Length > 0)
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
}
