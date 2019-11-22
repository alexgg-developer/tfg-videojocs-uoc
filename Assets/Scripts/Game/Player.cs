using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    UnitManager unitManager;
    CityManager cityManager;
    uint shields = 0;
    public uint Shields { get { return shields; } set { shields = value; } }
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

    public void ResetUnitMovement()
    {
        unitManager.ResetUnitMovement();
    }

    public void RemoveUnit(Unit unit)
    {
        unitManager.RemoveUnit(unit);
    }

    public uint CalculateShieldsPerTurn()
    {
        return cityManager.CalculateShieldsPerTurn();
    }

    public void ProduceShields()
    {
        Shields += CalculateShieldsPerTurn();
    }
}
