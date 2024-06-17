using UnityEngine;

public class ShowInteractionBox : MonoBehaviour
{
    [TextArea(3, 6)]
    [SerializeField] private string interactionText;
    public void ShowBox()
    {
        UserInterface.Instance.SetInteractionBox(interactionText);
    }
}
