using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static HexGrid;
using Random = UnityEngine.Random;

public class HexMapEditor : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private HexGrid hexGrid;
    [SerializeField]
    private int chunksX, chunksZ;
    [SerializeField]
    private int MaxElevation;
#pragma warning restore 0649

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    OptionalToggle riverMode;

    private Color[] colors;
    private Color activeColor;
    int activeElevation;
    bool applyColor = true;
    private bool applyElevation = true;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

    void Awake()
    {
        colors = new Color[(int)TerrainTypes.SIZE];
        colors[(int)TerrainTypes.GRASS] = Color.green;
        colors[(int)TerrainTypes.ICE] = Color.white;
        colors[(int)TerrainTypes.MOUNTAIN] = new Color(178f / 255f, 115f / 255f, 65f / 255f);
        colors[(int)TerrainTypes.PLAIN] = Color.yellow;
        colors[(int)TerrainTypes.WATER] = Color.cyan;

        SelectColor(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()) {
            HandleInput();
        }
        else {
            previousCell = null;
        }
    }

    void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell) {
                ValidateDrag(currentCell);
            }
            else {
                isDrag = false;
            }
            EditCell(currentCell);
            previousCell = currentCell;
        }
        else {
            previousCell = null;
        }
    }

    void ValidateDrag(HexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        ) 
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell) {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    public void CreateGrid()
    {
        hexGrid.CreateCells(chunksX, chunksZ);
        HexCell[] hexCells = hexGrid.Cells;
        int cellsInX = chunksX * HexMetrics.chunkSizeX;
        for (int i = 0; i < chunksZ * HexMetrics.chunkSizeZ; ++i) {
            for (int j = 0; j < cellsInX; ++j) {
                //hexCells[i * cellsInX + j].Color = colors[Random.Range(0, (int)TerrainTypes.SIZE)];
                hexCells[i * cellsInX + j].Color = colors[0];
                //hexCells[i * cellsInX + j].Elevation = Random.Range(0, MaxElevation);
                /*float random = Random.value;
                if (random <= 0.12f) {
                    hexCells[i * cellsInX + j].SetResource(true);
                }*/
            }
        }
        hexGrid.Refresh();
    }

    void EditCell(HexCell cell)
    {
		if (cell) {
			if (applyColor) {
				cell.Color = activeColor;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			else if (isDrag && riverMode == OptionalToggle.Yes) {
				previousCell.SetOutgoingRiver(dragDirection);
			}
		}
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor) {
            activeColor = colors[index];
        }
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
        hexGrid.Refresh();
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }
}
