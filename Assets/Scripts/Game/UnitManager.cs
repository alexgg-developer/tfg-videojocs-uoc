using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnitStats;

public class UnitManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject[] unitPrefabs;
    [SerializeField]
    GameObject[] unitPrefabsRed;
    GameObject[] currentUnitPrefabs;
    [SerializeField]
    HexGrid grid;
    [SerializeField]
    private Text healthStatsPrefab;

    public HexGrid Grid { get { return grid; } set { grid = value; } }
    int playerID = 0;
    public int PlayerID { get { return playerID; } set { playerID = value; } }

    List<GameObject> unitInstances = new List<GameObject>();
    public List<GameObject> Units { get { return unitInstances; } set { unitInstances = value; } }

    

    public void Awake()
    {
        
    }

    public void InstantiateInitialUnits(Tuple<int, int> initialPosition)
    {
        switch (playerID) {
            case 0:
                currentUnitPrefabs = unitPrefabs;
                break;
            case 1:
                currentUnitPrefabs = unitPrefabsRed;
                break;
        }

        int index = initialPosition.Item1 + initialPosition.Item2 * grid.CellCountX;
        AddUnit(index, UnitType.WARRIOR);
        //AddUnit(index, UnitType.HORSEMAN);
        //AddUnit(index, UnitType.ARCHER);
        //AddUnit(index, UnitType.CATAPULT);
        index = initialPosition.Item1 + (initialPosition.Item2 + 1) * grid.CellCountX;
        AddUnit(index, UnitType.SETTLER);
        //AddUnit(index, UnitType.CATAPULT);
        /*index = initialPosition.Item1 + (initialPosition.Item2 + 2) * grid.Width;
        AddUnit(index, UnitType.HORSEMAN);*/
    }

    public void RemoveUnit(Unit unit)
    {
        for (int i = 0; i < unitInstances.Count; ++i) {
            Unit currentUnit = unitInstances[i].GetComponent<Unit>();
            if (currentUnit.ID == unit.ID) {
                unitInstances.RemoveAt(i);
                break;
            }
        }
    }

    public void ResetUnitMovement()
    {
        foreach (var unitGO in unitInstances) {
            unitGO.GetComponent<Unit>().Reset();
        }
    }

    public void AddUnit(int index, UnitType type)
    {
        HexCell cell = grid.GetCell(index);
        AddUnitInCell(cell, type);
    }

    /*public Unit AddUnit(HexCoordinates hexCoordinates, UnitType type)
    {
        Tuple<int, int> offsetCoordinates = HexCoordinates.ToOffsetCoordinates(hexCoordinates);
        HexCell cell = grid.GetCell(offsetCoordinates.Item1, offsetCoordinates.Item2);
        return AddUnitInCell(cell, type);
    }*/

    public Unit AddUnitInCell(HexCell cell, UnitType type)
    {
        GameObject unitInstance = Instantiate(currentUnitPrefabs[(int)type], cell.transform);
        Unit unitComponent = unitInstance.GetComponent<Unit>();
        unitComponent.PlayerID = playerID;
        float offsetY = unitInstance.GetComponent<MeshFilter>().mesh.bounds.size.y * unitInstance.transform.localScale.y * 0.5f;
        if(type == UnitType.SHIP) {
            offsetY += cell.WaterSurfaceY;
        }
        unitInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        unitInstances.Add(unitInstance);

        unitComponent.OnNewPosition();

        return unitComponent;
    }

    internal bool HasSettler()
    {
        bool hasSettler = false;
        for (int i = 0; i < unitInstances.Count; ++i) {
            Unit currentUnit = unitInstances[i].GetComponent<Unit>();
            if (currentUnit.Type == UnitType.SETTLER) {
                hasSettler = true;
                break;
            }
        }
        return hasSettler;
    }
}
