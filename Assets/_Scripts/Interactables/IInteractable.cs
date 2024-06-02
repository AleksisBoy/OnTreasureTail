

using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Call for assigning interactable to Island Manager. Have to be called in Start
    /// </summary>
    public void AssignToIslandManager();

    /// <summary>
    /// Interaction call
    /// </summary>
    /// <returns>True if pause needed</returns>
    public bool Interact(); 
    public bool HasInteracted();
    public bool InteractionActive();
    public Vector3 GetPosition();
}
