using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellBasedViewport : MonoBehaviour
{
    [SerializeField] private CellUI cellPrefab = null;
    [SerializeField] private InfoTextCelledUI infoTextPrefab = null;
    [SerializeField] private Vector2 cellSize = new Vector2(25, 25);

    public Vector2 CellSize => cellSize;
    private List<Cell> cells = new List<Cell>();
    private List<CellUI> cellsUI = new List<CellUI>();
    public List<Cell> Cells => cells;
    private List<InfoCelled> celledInfo = new List<InfoCelled>();
    public List<InfoCelled> CelledInfo => celledInfo;
    private RectTransform RT { get { return (RectTransform)transform; } }
    private CellUI currentSelected = null;
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        CellUI selected = InsideCellUI(Input.mousePosition);
        if(selected == null) return;

        if (currentSelected && currentSelected != selected) currentSelected.Unselect();

        if (selected.Cell.occupied)
        {
            Debug.Log(selected.Cell.coordinates + " occupied");
        }
        else
        {
            currentSelected = selected;
        }
        
    }
    [ContextMenu("DestroyCellsUI")]
    private void DestroyCellsUI()
    {
        foreach(var cell in cellsUI)
        {
            DestroyImmediate(cell.gameObject);
        }
        cellsUI.Clear();
        cells.Clear();
    }
    [ContextMenu("Calculate")]
    public void BuildGrid()
    {
        cells.Clear();
        cellsUI.Clear();
        Vector2 pivotOffset = new Vector2(RT.rect.size.x * RT.pivot.x, RT.rect.size.y * RT.pivot.y);

        Vector3 worldCornerLeft = RT.TransformPoint(new Vector3(-pivotOffset.x, -pivotOffset.y, 0));
        Vector3 worldCornerRight = RT.TransformPoint(new Vector3(pivotOffset.x, pivotOffset.y, 0));

        int xINT = 0; int yINT = 0;
        for (float y = worldCornerRight.y - cellSize.y; y > worldCornerLeft.y; y -= cellSize.y)
        {
            for (float x = worldCornerLeft.x; x < worldCornerRight.x; x += cellSize.x)
            {
                Vector2 cellCornerEnd = new Vector2(x + cellSize.x, y + cellSize.y);
                if (cellCornerEnd.x > worldCornerRight.x) continue;
                if (cellCornerEnd.y > worldCornerRight.y) continue;

                Cell cell = new Cell(new Vector2(x, y), cellCornerEnd, new Vector2Int(xINT, yINT));
                CellUI cellUI = Instantiate(cellPrefab, transform);

                cellUI.Set(cell);
                cellsUI.Add(cellUI);
                cells.Add(cell);
                xINT++;
            }
            yINT++;
            xINT = 0;
        }
    }
    public Cell CheckInsideCell(Vector3 pos)
    {
        Cell cell = InsideCell(pos);
        return cell;
    }
    public Cell InsideCell(Vector3 pixelPosition)
    {
        List<Cell> cellsXSorted = cells.Where(cell => cell.cornerEnd.x > pixelPosition.x && cell.cornerStart.x <= pixelPosition.x).ToList();
        List<Cell> cellsSorted = cellsXSorted.Where(cell => cell.cornerEnd.y > pixelPosition.y && cell.cornerStart.y <= pixelPosition.y).ToList();

        if(cellsSorted.Count != 1) return null;
        
        return cellsSorted[0];
    }
    public CellUI InsideCellUI(Vector3 pixelPosition)
    {
        List<Cell> cellsXSorted = cells.Where(cell => cell.cornerEnd.x > pixelPosition.x && cell.cornerStart.x <= pixelPosition.x).ToList();
        List<Cell> cellsSorted = cellsXSorted.Where(cell => cell.cornerEnd.y > pixelPosition.y && cell.cornerStart.y <= pixelPosition.y).ToList();

        if(cellsSorted.Count != 1) return null;

        CellUI cellUI = cellsUI.FirstOrDefault(cellUI => cellUI.Cell == cellsSorted[0]);

        return cellUI;
    }
    public bool TryAddTextToViewport(Vector2Int[] objectSize, InfoTextSO text)
    {
        int i = 0;
        List<Cell> cellList = new List<Cell>();
        while (cellList.Count != objectSize.Length)
        {
            cellList.Clear();
            foreach (var coor in objectSize)
            {
                Cell cell = GetCell(cells[i].coordinates + coor);
                if (cell == null || cell.occupied) break;

                cellList.Add(cell);
            }
            i++;
            if (i >= cells.Count)
            {
                cellList.Clear();
                break;
            }
        }
        foreach(Cell cell in cellList)
        {
            cell.occupied = true;
            Debug.Log(cell);
        }
        InfoCelled infoCelled = new InfoCelled(text, cellList);
        InfoTextCelledUI infoCelledUI = Instantiate(infoTextPrefab, transform);
        infoCelledUI.Set(infoCelled);
        celledInfo.Add(infoCelled);
        return true;
    }
    private Cell GetCell(Vector2Int coordinates)
    {
        return cells.FirstOrDefault(cell => cell.coordinates == coordinates);
    }
}
public class Cell
{
    public bool occupied = false;
    public Vector2Int coordinates;
    public Vector2 cornerStart;
    public Vector2 cornerEnd;
    public Cell(Vector2 corner1, Vector2 corner2, Vector2Int coordinates)
    {
        cornerStart = corner1;
        cornerEnd = corner2;
        this.coordinates = coordinates;
        occupied = false;
    }
}
public class InfoCelled
{
    public Vector2 cornerStart;
    public Vector2 cornerEnd;
    public InfoSO info = null;
    public List<Cell> cells = new List<Cell>();
    public InfoCelled(InfoSO info, List<Cell> cells)
    {
        this.info = info;
        this.cells = cells;
        UpdateCorners();
    }
    private void UpdateCorners()
    {
        cornerStart = cells[0].cornerStart;
        cornerEnd = cells[0].cornerEnd;
        foreach (Cell cell in cells)
        {
            if(cornerStart.x > cell.cornerStart.x)
            {
                cornerStart.x = cell.cornerStart.x;
            }
            else if(cornerEnd.x < cell.cornerEnd.x)
            {
                cornerEnd.x = cell.cornerEnd.x;
            }
            if(cornerStart.y > cell.cornerStart.y)
            {
                cornerStart.y = cell.cornerStart.y;
            }
            else if(cornerEnd.y < cell.cornerEnd.y)
            {
                cornerEnd.y = cell.cornerEnd.y;
            }
        }
    }
    public Vector2 GetSize()
    {
        return new Vector2(cornerEnd.x - cornerStart.x, cornerEnd.y - cornerStart.y);
    }
}