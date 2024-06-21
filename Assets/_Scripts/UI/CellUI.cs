using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    [SerializeField] private Image cellImage = null;

    private Cell cell = null;

    public Cell Cell => cell;
    public RectTransform RT { get { return (RectTransform)transform; } }
    public void Set(Cell cell)
    {
        this.cell = cell;
        RT.sizeDelta = new Vector2(cell.positionRect.end.x - cell.positionRect.start.x, cell.positionRect.end.y - cell.positionRect.start.y);
        RT.position = cell.positionRect.start + new Vector2(RT.sizeDelta.x / 2, RT.sizeDelta.y / 2);
        Unselect();
    }
    public void Select()
    {
        cellImage.color = InternalSettings.SelectedCellColor;
    }
    public void Unselect()
    {
        cellImage.color = InternalSettings.DefaultCellColor;
    }
}
