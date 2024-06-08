using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable, IInteractable
{
    [Header("Door")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private bool disableAfterOpen = false;
    [SerializeField] private bool locked = false;

    private bool open = false;
    protected override void Awake()
    {
        base.Awake();

        open = false;
        animator.SetBool("Open", false);
    }
    private void Start()
    {
        AssignToIslandManager();
    }
    private void OpenToggle()
    {
        open = !open;
        animator.SetBool("Open", open);
        if (disableAfterOpen && open)
        {
            enabled = false;
        }
    }
    public void Unlock()
    {
        locked = false;
    }
    // IInteractable calls
    public void Interact()
    {
        if (locked)
        {
            Debug.Log(name + " is locked");
        }
        else
        {
            OpenToggle();
        }
    }
    public void AssignToIslandManager()
    {
        IslandManager.Instance.AssignInteractable(this);
    }


    // Getters
    public bool HasInteracted()
    {
        return interacted;
    }

    public Vector3 GetInteractionPosition()
    {
        return interactionPoint.position;
    }

    public float GetInteractionDistance()
    {
        return interactionDistance;
    }
    public bool InteractionActive()
    {
        return gameObject.activeSelf && enabled;
    }
}
