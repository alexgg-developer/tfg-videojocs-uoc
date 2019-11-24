using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public const uint SHIELDS_PER_POPULATION = 2;

    private static int nextID = 0;

    private int id;
    public int ID { get { return id; } set { id = value; } }
    private uint population = 1;
    public uint Population { get { return population; } set { population = value; } }
    private int playerID;
    public int PlayerID { get { return playerID; } set { playerID = value; } }
    private string name = "";
    public string Name { get { return name; } set { name = value; } }

    private HashSet<BuildingInfo.BuildingType> buildingsBuilt = new HashSet<BuildingInfo.BuildingType>();

    
    void Awake()
    {
        ID = nextID++;
    }

    public uint ShieldsPerTurn()
    {
        return population * City.SHIELDS_PER_POPULATION;
    }

    internal void BuildBuilding(BuildingInfo.BuildingType buildingType)
    {
        buildingsBuilt.Add(buildingType);
        if (buildingType == BuildingInfo.BuildingType.BARN) ++Population;
    }

    internal bool HasBuilding(BuildingInfo.BuildingType buildingType)
    {
        return buildingsBuilt.Contains(buildingType);
    }

    public bool HasWall()
    {
        return buildingsBuilt.Contains(BuildingInfo.BuildingType.WALL);
    }
}
