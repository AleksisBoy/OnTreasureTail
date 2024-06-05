using System;
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
    private List<InfoCelledUI> celledInfoUI = new List<InfoCelledUI>();
    private RectTransform RT { get { return (RectTransform)transform; } }
    private InfoCelledUI currentDragged = null;
    private Vector2Int[] Directions = new Vector2Int[4]
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(-1, 0), new Vector2Int(0, 1)
    };
    private void Start()
    {
        currentDragged = null;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InfoCelledUI cellOnMouse = InsideInfoCellUI(Input.mousePosition);
            if(cellOnMouse)
            {
                currentDragged = cellOnMouse;
                currentDragged.transform.SetAsLastSibling();
            }
        }
        else if (Input.GetMouseButton(0) && currentDragged)
        {
            currentDragged.RT.position = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && currentDragged)
        {
            if(InsideCell(Input.mousePosition) != null)
            {
                foreach (Cell cell in currentDragged.InfoCelled.cells)
                {
                    cell.occupied = false;
                }
                List<Cell> cellList = GetFitCellsNearest(InsideCell(Input.mousePosition), currentDragged.InfoCelled.info.ObjectSize);
                foreach (Cell cell in cellList)
                {
                    cell.occupied = true;
                    Debug.Log(cell.coordinates);
                }

                currentDragged.UpdateInfoCells(cellList);
                currentDragged.PlaceOnBasePosition();
            }
            else
            {
                currentDragged.PlaceOnBasePosition();
            }
            currentDragged = null;
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
        List<Cell> cellsXSorted = cells.Where(cell => cell.positionRect.end.x > pixelPosition.x && cell.positionRect.start.x <= pixelPosition.x).ToList();
        List<Cell> cellsSorted = cellsXSorted.Where(cell => cell.positionRect.end.y > pixelPosition.y && cell.positionRect.start.y <= pixelPosition.y).ToList();

        if(cellsSorted.Count != 1) return null;
        
        return cellsSorted[0];
    }
    public CellUI InsideCellUI(Vector3 pixelPosition)
    {
        List<Cell> cellsXSorted = cells.Where(cell => cell.positionRect.end.x > pixelPosition.x && cell.positionRect.start.x <= pixelPosition.x).ToList();
        List<Cell> cellsSorted = cellsXSorted.Where(cell => cell.positionRect.end.y > pixelPosition.y && cell.positionRect.start.y <= pixelPosition.y).ToList();

        if(cellsSorted.Count != 1) return null;

        CellUI cellUI = cellsUI.FirstOrDefault(cellUI => cellUI.Cell == cellsSorted[0]);

        return cellUI;
    }
    public InfoCelledUI InsideInfoCellUI(Vector3 pixelPosition)
    {
        Debug.Log(celledInfoUI.Count);
        List<InfoCelledUI> cellsXSorted = celledInfoUI.Where(cell => cell.InfoCelled.positionRect.end.x > pixelPosition.x && cell.InfoCelled.positionRect.start.x <= pixelPosition.x).ToList();
        List<InfoCelledUI> cellsSorted = cellsXSorted.Where(cell => cell.InfoCelled.positionRect.end.y > pixelPosition.y && cell.InfoCelled.positionRect.start.y <= pixelPosition.y).ToList();

        if(cellsSorted.Count != 1) return null;

        InfoCelledUI cellUI = celledInfoUI.FirstOrDefault(cellUI => cellUI == cellsSorted[0]);

        return cellUI;
    }
    public bool TryAddTextToViewport(Vector2Int[] objectSize, InfoTextSO text)
    {
        List<Cell> cellList = GetFitCellsFromBase(objectSize);
        foreach (Cell cell in cellList)
        {
            cell.occupied = true;
            Debug.Log(cell);
        }
        InfoCelled infoCelled = new InfoCelled(text, cellList);
        InfoTextCelledUI infoCelledUI = Instantiate(infoTextPrefab, transform);
        infoCelledUI.Set(infoCelled);
        celledInfoUI.Add(infoCelledUI);
        celledInfo.Add(infoCelled);
        return true;
    }

    private List<Cell> GetFitCellsFromBase(Vector2Int[] objectSize)
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

        return cellList;
    }
    private List<Cell> GetFitCellsNearest(Cell startCell, Vector2Int[] objectSize)
    {
        Queue<Cell> search = new Queue<Cell>();

        List<Cell> cellList = new List<Cell>();

        foreach (var coor in objectSize)
        {
            Cell cell = GetCell(startCell.coordinates + coor);
            if (cell == null || cell.occupied) break;

            cellList.Add(cell);
        }
        if (cellList.Count != objectSize.Length)
        {
            cellList.Clear();
            search.Enqueue(startCell);
        }
        while (search.Count > 0)
        {
            Cell checkedCell = search.Dequeue();
            for(int i = 0; i < 4; i++)
            {
                Cell adjCell = GetCell(checkedCell.coordinates + Directions[i]);
                if (adjCell == null) continue;

                foreach (var coor in objectSize)
                {
                    Cell cell = GetCell(adjCell.coordinates + coor);
                    if (cell == null || cell.occupied) break;

                    cellList.Add(cell);
                }
                if (cellList.Count != objectSize.Length)
                {
                    cellList.Clear();
                    search.Enqueue(adjCell);
                }
                else
                {
                    search.Clear();
                    break;
                }
            }
        }

        return cellList;
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
    public Vector2Range positionRect;
    public Cell(Vector2 corner1, Vector2 corner2, Vector2Int coordinates)
    {
        positionRect = new Vector2Range();
        positionRect.start = corner1;
        positionRect.end = corner2;
        this.coordinates = coordinates;
        occupied = false;
    }
}
public class InfoCelled
{
    public Vector2Range positionRect;
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
        positionRect = new Vector2Range();
        positionRect.start = cells[0].positionRect.start;
        positionRect.end = cells[0].positionRect.end;
        foreach (Cell cell in cells)
        {
            if(positionRect.start.x > cell.positionRect.start.x)
            {
                positionRect.start.x = cell.positionRect.start.x;
            }
            else if(positionRect.end.x < cell.positionRect.end.x)
            {
                positionRect.end.x = cell.positionRect.end.x;
            }
            if(positionRect.start.y > cell.positionRect.start.y)
            {
                positionRect.start.y = cell.positionRect.start.y;
            }
            else if(positionRect.end.y < cell.positionRect.end.y)
            {
                positionRect.end.y = cell.positionRect.end.y;
            }
        }
    }
    public Vector2 GetSize()
    {
        return new Vector2(positionRect.end.x - positionRect.start.x, positionRect.end.y - positionRect.start.y);
    }
}
[Serializable]
public struct Vector2Range
{
    public Vector2 start;
    public Vector2 end;
}