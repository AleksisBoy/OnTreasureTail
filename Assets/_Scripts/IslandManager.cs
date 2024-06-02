using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    private List<IInteractable> interactables = new List<IInteractable>();
    public static IslandManager Instance { get; private set; } = null;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public bool AssignInteractable(IInteractable interactable)
    {
        if(interactables.Contains(interactable)) return false;
        interactables.Add(interactable);
        return true;
    }
    public IInteractable GetClosestInteractable(Vector3 position)
    {
        IInteractable closestInteractable = null;
        float closestDistance = 1000f;
        foreach(IInteractable interactable in interactables)
        {
            float distance = Vector3.Distance(interactable.GetPosition(), position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }
    public IInteractable GetClosestInteractable(Vector3 position, out float closestDistance)
    {
        IInteractable closestInteractable = null;
        closestDistance = 1000f;
        foreach(IInteractable interactable in interactables)
        {
            float distance = Vector3.Distance(interactable.GetPosition(), position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
