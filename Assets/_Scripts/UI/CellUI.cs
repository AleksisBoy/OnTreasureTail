using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    [SerializeField] private Image cellImage = null;

    private Cell cell = null;

    public Cell Cell => cell;
    private RectTransform RT { get { return (RectTransform)transform; } }
    public void Set(Cell cell)
    {
        this.cell = cell;
        RT.sizeDelta = new Vector2(cell.cornerEnd.x - cell.cornerStart.x, cell.cornerEnd.y - cell.cornerStart.y);
        RT.position = cell.cornerStart + new Vector2(RT.sizeDelta.x / 2, RT.sizeDelta.y / 2);
        Unselect();
    }
    public void Select()
    {
        cellImage.color = InternalSettings.Get.SelectedCellColor;
    }
    public void Unselect()
    {
        cellImage.color = InternalSettings.Get.DefaultCellColor;
    }
}
