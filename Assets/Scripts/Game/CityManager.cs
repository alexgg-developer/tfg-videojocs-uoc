using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject[] cityPrefabs;
    [SerializeField]
    HexGrid grid;
    public HexGrid Grid { get { return grid; } set { grid = value; } }
    int playerID = 0;
    public int PlayerID { get { return playerID; } set { playerID = value; } }

    List<GameObject> cityInstances = new List<GameObject>();

    public void AddCity(int index)
    {
        HexCell cell = grid.GetCell(index);
        GameObject cityInstance = Instantiate(cityPrefabs[0], cell.transform);
        City cityComponent = cityInstance.GetComponent<City>();
        cityComponent.PlayerID = playerID;
        float offsetY = cityInstance.GetComponent<MeshFilter>().mesh.bounds.size.y * cityInstance.transform.localScale.y * 0.5f;
        cityInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        cityInstances.Add(cityInstance);
    }

    public void AddCity(HexCoordinates hexCoordinates)
    {
        HexCell cell = grid.GetCell(hexCoordinates.X, hexCoordinates.Z);
        GameObject unitInstance = Instantiate(cityPrefabs[0], cell.transform);
        float offsetY = unitInstance.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y * unitInstance.transform.localScale.y * 0.5f;
        unitInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        cityInstances.Add(unitInstance);
    }
}
