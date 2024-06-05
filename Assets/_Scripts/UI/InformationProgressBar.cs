using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InformationProgressBar : MonoBehaviour, IPointerMoveHandler
{
    [SerializeField] private GUIStyle cellStyle = null;
    [SerializeField] private GUIStyle infoStyle = null;
    [SerializeField] private Color cellColor = Color.white;
    [SerializeField] private Color selectedCellColor = Color.white;
    [SerializeField] private CellBasedViewport viewport = null;

    private List<InfoSO> infoInBar = new List<InfoSO>();

    private Cell currentCell = null;
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
    private void OnGUI()
    {
        GUI.color = cellColor;
        foreach (Cell cell in viewport.Cells)
        {
            if (cell.occupied) continue;
            GUI.Box(new Rect(new Vector2(cell.positionRect.start.x, Mathf.Abs(cell.positionRect.end.y - Screen.height)), viewport.CellSize),"", cellStyle);

        }
        GUI.color = Color.blue;
        foreach (InfoCelled infoCelled in viewport.CelledInfo)
        {
            foreach(Cell cell in infoCelled.cells)
            {
                if (cell == currentCell)
                {
                    GUI.color = Color.red;
                    break;
                }
            }
            GUI.Box(new Rect(new Vector2(infoCelled.positionRect.start.x, Mathf.Abs(infoCelled.positionRect.end.y - Screen.height)), infoCelled.GetSize()), "INFO", infoStyle);

            GUI.color = Color.blue;
        }
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        currentCell = viewport.CheckInsideCell(eventData.position);
    }
}
