﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static HexGrid;
using static MapResource;
using Random = UnityEngine.Random;
using System.IO;
using UnityEngine.SceneManagement;

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

    int activeTerrainTypeIndex;
    int activeElevation;
    int activeWaterLevel;
    int activeResource;

    private bool applyElevation = true;
    private bool applyWaterLevel = true;
    private bool applyResource = false;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

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
                hexCells[i * cellsInX + j].TerrainType = TerrainTypes.GRASS;
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
            if (activeTerrainTypeIndex >= 0) {
                cell.TerrainType = (TerrainTypes)activeTerrainTypeIndex;
            }
            if (applyElevation) {
				cell.Elevation = activeElevation;
			}
            if (applyWaterLevel) {
                cell.WaterLevel = activeWaterLevel;
            }

            if(applyResource) {
                cell.SetResource((ResourceKind)activeResource);
            }
            else {
                cell.SetResource(ResourceKind.NONE);
            }

            if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			else if (isDrag && riverMode == OptionalToggle.Yes) {
				previousCell.SetOutgoingRiver(dragDirection);
			}
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

    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    public void SetApplyResource(bool toggle)
    {
        applyResource = toggle;
    }

    public void SetResource(int resource)
    {
        activeResource = resource;
    }

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }

    public void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        //Debug.Log(path);
        using (BinaryWriter writer =
                new BinaryWriter(File.Open(path, FileMode.Create))) 
        {
			hexGrid.Save(writer);
        }
        OpenInWin(path);
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (BinaryReader reader =
                new BinaryReader(File.OpenRead(path))
        ) 
        {
            hexGrid.Load(reader);
        }
    }


    public static void OpenInWin(string path)
    {
        bool openInsidesOfFolder = false;

        // try windows
        string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

        if (System.IO.Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        try {
            System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
        }
        catch (System.ComponentModel.Win32Exception e) {
            // tried to open win explorer in mac
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
        PlayerPrefs.SetInt("SceneToLoad", 0);
    }

    public void Exit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
