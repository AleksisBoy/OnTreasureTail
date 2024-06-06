using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationBar : MonoBehaviour
{
    [SerializeField] private CellBasedViewport viewport = null;

    private List<InfoSO> infoInBar = new List<InfoSO>();
    public void AddInfoToBar(InfoSO newInfo)
    {
        infoInBar.Add(newInfo);
        switch (newInfo.GetType().ToString())
        {
            case "InfoTextSO": viewport.TryAddTextToViewport(newInfo.ObjectSize, (InfoTextSO)newInfo); break;
            //case "InfoPictureSO": viewport.TryAddTextToViewport(); break;
            default: break;
        }
    }
    public void BuildGridViewport()
    {
        viewport.BuildGrid();
    }
    public void SetViewportDragObjects(ICellDragEnd[] objects)
    {
        viewport.SetDragEndObjects(objects);
    }
}
