

using UnityEngine;

public interface IInteractable
{
    public void AssignToIslandManager();
    public bool Interact();
    public bool HasInteracted();
    public bool InteractionActive();
    public Vector3 GetPosition();
}
