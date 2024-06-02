using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement = null;
    [SerializeField] private PlayerCamera view = null;
    [SerializeField] private float interactionDistance = 0.4f;

    private void Update()
    {
        InteractionInput();
    }

    private void InteractionInput()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        IInteractable inter = IslandManager.Instance.GetClosestInteractable(transform.position, out float distance);

        if (inter != null && distance < interactionDistance)
        {
            bool pause = inter.Interact();
            if (pause) EnablePlayerComponents(false);
        }
    }

    public void EnablePlayerComponents(bool state)
    {
        movement.enabled = state;
        view.enabled = state;
        this.enabled = state;
    }
}
