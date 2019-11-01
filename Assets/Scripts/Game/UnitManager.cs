using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitStats;

public class UnitManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject[] unitPrefabs;
    [SerializeField]
    HexGrid grid;
    public HexGrid Grid { get { return grid; } set { grid = value; } }
    int playerID = 0;
    public int PlayerID { get { return playerID; } set { playerID = value; } }

    List<GameObject> unitInstances = new List<GameObject>();

    public void InstantiateIntialUnits(Tuple<int, int> initialPosition)
    {
        int index = initialPosition.Item1 + initialPosition.Item2 * grid.Width;
        AddUnit(index, UnitType.WARRIOR);
        index = initialPosition.Item1 + (initialPosition.Item2 + 1) * grid.Width;
        AddUnit(index, UnitType.SETTLER);
    }

    public void RemoveUnit(Unit unit)
    {
        for(int i = 0; i < unitInstances.Count; ++i) { 
            Unit currentUnit = unitInstances[i].GetComponent<Unit>();
            if(currentUnit.PlayerID == unit.PlayerID) {
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
        GameObject unitInstance = Instantiate(unitPrefabs[(int)type], cell.transform);
        Unit unitComponent = unitInstance.GetComponent<Unit>();
        unitComponent.PlayerID = playerID;
        float offsetY = unitInstance.GetComponent<MeshFilter>().mesh.bounds.size.y * unitInstance.transform.localScale.y * 0.5f;
        unitInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        unitInstances.Add(unitInstance);
    }

    public void AddUnit(HexCoordinates hexCoordinates, UnitType type)
    {
        Tuple<int, int> offsetCoordinates = HexCoordinates.ToOffsetCoordinates(hexCoordinates);
        HexCell cell = grid.GetCell(offsetCoordinates.Item1, offsetCoordinates.Item2);
        GameObject unitInstance = Instantiate(unitPrefabs[(int)type], cell.transform);
        float offsetY = unitInstance.GetComponent<MeshFilter>().mesh.bounds.size.y * unitInstance.transform.localScale.y * 0.5f;
        unitInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        unitInstances.Add(unitInstance);
    }
}
