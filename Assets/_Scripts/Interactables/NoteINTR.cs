using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteINTR : Interactable, IInteractable
{
    [SerializeField] private TailPanel notePanel = null;

    private void Start()
    {
        AssignToIslandManager();
    }
    // IInteractable calls
    public void AssignToIslandManager()
    {
        IslandManager.Instance.AssignInteractable(this);
    }

    public bool HasInteracted()
    {
        return interacted;
    }

    public bool Interact()
    {
        UIManager.Open(notePanel, OnCloseNote);
        interacted = true;
        return true;
    }
    private void OnCloseNote()
    {
        if (!UIManager.IsOpen()) PlayerInteraction.Instance.EnablePlayerComponents(true);
    }
    // Getters
    public bool InteractionActive()
    {
        return gameObject.activeSelf;
    }
    public Vector3 GetInteractionPosition()
    {
        return interactionPoint.position;
    }
    public float GetInteractionDistance()
    {
        return interactionDistance;
    }
}
