using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    [SerializeField] protected Transform interactionPoint = null;

    protected bool interacted = false;

    protected virtual void Awake()
    {
        if (interactionPoint == null)
        {
            interactionPoint = new GameObject("InteractionPoint").transform;
            interactionPoint.SetParent(transform, false);
        }
    }
}
