using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoProviderINTR : Interactable, IInteractable
{
    [Header("Info Provider")]
    [SerializeField] private InfoSO infoProvided = null;

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

    public void Interact()
    {
        if (!interacted)
        {
            AcquireWords();
            interacted = true;
        }
    }
    private void AcquireWords()
    {
        Debug.Log("acquired " + infoProvided.name);
        IslandManager.Instance.AddInfo(infoProvided);
    }

    public bool InteractionActive()
    {
        return gameObject.activeSelf;
    }

    // Getters
    public Vector3 GetInteractionPosition()
    {
        return interactionPoint.position;
    }

    public float GetInteractionDistance()
    {
        return interactionDistance;
    }
}
