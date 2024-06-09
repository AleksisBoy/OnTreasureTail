using UnityEngine;

public class InfoProvideObject : MonoBehaviour
{
    [SerializeField] private InfoSO[] infoProvided = null;

    private bool provided = false;
    private void OnMouseDown()
    {
        if (!provided && PlayerCamera.Instance.FocusCamera.gameObject.activeSelf)
        {
            provided = true;
            IslandManager.Instance.AddInfo(Input.mousePosition, infoProvided);
        }
    }
}
