using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitStats;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] unitPrefabs;
    [SerializeField]
    HexGrid grid;

    List<GameObject> unitInstances = new List<GameObject>();

    public void InstantiateIntialUnits()
    {
        /*Tuple<int, int> offsetCoordinates = HexCoordinates.ToOffsetCoordinates(new HexCoordinates(0, 1));
        HexCell cell = grid.GetCell(offsetCoordinates.Item1, offsetCoordinates.Item2);
        //unitInstances.Add(Instantiate(unitPrefabs[(int)UnitType.WARRIOR], new Vector3(0, 0, 0), Quaternion.identity));
        GameObject unitInstance = Instantiate(unitPrefabs[(int)UnitType.WARRIOR], cell.transform);
        float offsetY = unitInstance.GetComponent<MeshFilter>().mesh.bounds.size.y * unitInstance.transform.localScale.y * 0.5f;
        unitInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        unitInstances.Add(unitInstance);

        offsetCoordinates = HexCoordinates.ToOffsetCoordinates(new HexCoordinates(0, 0));
        cell = grid.GetCell(offsetCoordinates.Item1, offsetCoordinates.Item2);
        unitInstance = Instantiate(unitPrefabs[(int)UnitType.SETTLER], cell.transform);
        offsetY = unitInstance.GetComponent<MeshFilter>().mesh.bounds.size.y * unitInstance.transform.localScale.y * 0.5f;
        unitInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        unitInstances.Add(unitInstance);*/

        AddUnit(new HexCoordinates(0, 1), UnitType.WARRIOR);
        AddUnit(new HexCoordinates(0, 2), UnitType.WARRIOR);
        AddUnit(new HexCoordinates(0, 0), UnitType.SETTLER);
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
