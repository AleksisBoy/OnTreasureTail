using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvideInfo : MonoBehaviour
{
    [SerializeField] private InfoSO[] infoProvided = null;

    private bool provided = false;
    public void CallProvideInfo()
    {
        if (!provided)
        {
            provided = true;
            IslandManager.Instance.AddInfo(Input.mousePosition, infoProvided);
        }
    }
}
