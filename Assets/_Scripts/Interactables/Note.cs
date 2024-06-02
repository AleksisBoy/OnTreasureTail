using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [SerializeField] private TailPanel notePanel = null;

    private bool interacted = false;

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
        if (!UIManager.IsOpen()) FindObjectOfType<PlayerInteraction>().EnablePlayerComponents(true);
    }
    public bool InteractionActive()
    {
        return gameObject.activeSelf;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
