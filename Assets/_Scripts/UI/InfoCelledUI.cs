using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InfoCelledUI : MonoBehaviour
{
    protected InfoCelled infoCelled = null;
    private bool isLocked = false;
    public RectTransform RT { get { return (RectTransform)transform; } }
    public InfoCelled InfoCelled => infoCelled;
    public bool IsLocked => isLocked;

    public virtual void Set(InfoCelled infoCelled)
    {
        this.infoCelled = infoCelled;
        name = "InfoCelledUI_" + infoCelled.info.name;

        RT.sizeDelta = new Vector2(infoCelled.positionRect.end.x - infoCelled.positionRect.start.x, infoCelled.positionRect.end.y - infoCelled.positionRect.start.y);
        PlaceOnBasePosition();
    }
    public void PlaceOnBasePosition()
    {
        RT.position = infoCelled.positionRect.start + new Vector2(RT.sizeDelta.x / 2, RT.sizeDelta.y / 2);
    }
    public void SetPositionRect(Vector3 worldPos)
    {
        Debug.Log("new pos " + name);
        infoCelled.positionRect.start = (Vector2)worldPos - RT.sizeDelta / 2f;
        infoCelled.positionRect.end = (Vector2)worldPos + RT.sizeDelta / 2f;
    }
    public void UpdateCells(List<Cell> cells)
    {
        if (cells.Count != infoCelled.info.ObjectSize.Length) return;

        Vector2Range positionRect = cells[0].positionRect;
        foreach(Cell cell in cells)
        {
            if (positionRect.start.x > cell.positionRect.start.x)
            {
                positionRect.start.x = cell.positionRect.start.x;
            }
            if (positionRect.start.y > cell.positionRect.start.y)
            {
                positionRect.start.y = cell.positionRect.start.y;
            }
            if (positionRect.end.x < cell.positionRect.end.x)
            {
                positionRect.end.x = cell.positionRect.end.x;
            }
            if (positionRect.end.y < cell.positionRect.end.y)
            {
                positionRect.end.y = cell.positionRect.end.y;
            }
        }
        infoCelled.cells = cells;
        InfoCelled.positionRect = positionRect;
    }
    public void Lock(Vector3 worldPos)
    {
        isLocked = true;
        SetPositionRect(worldPos);
        PlaceOnBasePosition();
        gameObject.SetActive(false);
    }
}
