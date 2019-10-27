using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    UnitManager unitManager;
    CityManager cityManager;
    int score = 0;
    public int Score { get { return score; } set { score = value; } }
    public int PlayerID { get { return unitManager.PlayerID; } set { unitManager.PlayerID = value; } }


    public Player(UnitManager aUnitManager, CityManager aCityManager)
    {
        this.unitManager = aUnitManager;
        this.cityManager = aCityManager;
    }

    internal void InstantiateIntialUnits(Tuple<int, int> initialPosition)
    {
        unitManager.InstantiateIntialUnits(initialPosition);
    }

    public void SetGrid(HexGrid hexGrid)
    {
        unitManager.Grid = hexGrid;
        cityManager.Grid = hexGrid;
    }

    public void AddCity(HexCoordinates hexCoordinates)
    {
        cityManager.AddCity(hexCoordinates);
    }
}
