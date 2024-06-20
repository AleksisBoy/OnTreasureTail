using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventINTR : Interactable, IInteractable
{
    [SerializeField] private UnityEvent onInteract = null;
    [SerializeField] private bool repeatable = false;

    private void Start()
    {
        AssignToIslandManager();
    }
    public void AssignToIslandManager()
    {
        IslandManager.Instance.AssignInteractable(this);
    }

    public void Interact()
    {
        if (interacted) return;

        if (!repeatable) interacted = true;
        onInteract.Invoke();
    }


    // Getters
    public bool HasInteracted()
    {
        return interacted;
    }
    public bool InteractionActive()
    {
        return gameObject.activeSelf;
    }

    public float GetInteractionDistance()
    {
        return interactionDistance;
    }
    public Vector3 GetInteractionPosition()
    {
        return interactionPoint.position;
    }
}
